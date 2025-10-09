using Microsoft.AspNetCore.Mvc;
//using Project1.Services;
using Project1.Models.ViewModels;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Project1.Controllers
{
    public class ChatbotController : Controller
    {
        private readonly ChatbotApiService _chatbotApiService;
        private readonly IConfiguration _configuration;

        public ChatbotController(ChatbotApiService chatbotApiService, IConfiguration configuration)
        {
            _chatbotApiService = chatbotApiService;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var chatSession = _chatbotApiService.GetChatSession();
            ViewBag.Username = chatSession.Username;
            ViewBag.Gender = chatSession.Gender;
            ViewBag.ChatSession = chatSession;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string userQuestion, string username, string gender)
        {
            if (string.IsNullOrWhiteSpace(userQuestion))
            {
                ModelState.AddModelError("userQuestion", "Xin hãy nhập câu hỏi");
                var chatSessionLocal = _chatbotApiService.GetChatSession();
                ViewBag.Username = chatSessionLocal.Username;
                ViewBag.Gender = chatSessionLocal.Gender;
                ViewBag.ChatSession = chatSessionLocal;
                return View();
            }

            // Update user information
            _chatbotApiService.UpdateUserInfo(username, gender);

            // Add user message to chat history
            _chatbotApiService.AddUserMessage(username, userQuestion);

            // Get chat history for API payload
            var chatSession = _chatbotApiService.GetChatSession();
            var chatHistory = chatSession.Messages
                .Select(m => new { FromUser = m.MessageType == "user", Message = m.Message })
                .ToArray();


            var apiKey = _configuration["ChatbotApi:ApiKey"];
            var payload = new
            {
                ChatHistory = chatHistory,
                Question = userQuestion,
                ImagesAsBase64 = new string[0]
            };

            // Call the API
            string botAnswer;
            try
            {
                var responseJson = await _chatbotApiService.GenerateAnswerAsync(
                    apiKey, payload, username, gender, 18, 1, false, false);

                // Parse the JSON response
                botAnswer = ParseBotResponse(responseJson);
            }
            catch (Exception ex)
            {
                botAnswer = $"Tôi xin lỗi, nhưng đang có vấn đề xử lý yêu cầu của bạn. Error: {ex.Message}";
            }

            // Add bot message to chat history
            _chatbotApiService.AddBotMessage(botAnswer);

            // Refresh chat session for view
            var chatSessionRefreshed = _chatbotApiService.GetChatSession();
            ViewBag.Username = username;
            ViewBag.Gender = gender;
            ViewBag.UserQuestion = userQuestion; // For the form
            ViewBag.BotAnswer = botAnswer; // For the form
            ViewBag.ChatSession = chatSessionRefreshed;

            return View();
        }

        private string ParseBotResponse(string responseJson)
        {
            try
            {
                using var document = JsonDocument.Parse(responseJson);
                var root = document.RootElement;

                if (root.TryGetProperty("MessageInMarkdown", out var messageProperty))
                {
                    var markdown = messageProperty.GetString() ?? responseJson;

                    // Convert markdown to HTML for better display
                    return ConvertMarkdownToHtml(markdown);
                }

                return responseJson; // Fallback to raw response
            }
            catch
            {
                return responseJson; // Fallback to raw response if parsing fails
            }
        }

        private string ConvertMarkdownToHtml(string markdown)
        {
            if (string.IsNullOrEmpty(markdown))
                return markdown;

            // Simple markdown to HTML conversion
            var html = markdown
                .Replace("**", "<strong>")
                .Replace("\n", "<br>")
                .Replace("1. ", "<br>1. ")
                .Replace("2. ", "<br>2. ")
                .Replace("3. ", "<br>3. ")
                .Replace("4. ", "<br>4. ");

            return html;
        }

        [HttpPost]
        public IActionResult ClearChat()
        {
            _chatbotApiService.ClearChat();

            var chatSession = _chatbotApiService.GetChatSession();
            ViewBag.Username = chatSession.Username;
            ViewBag.Gender = chatSession.Gender;
            ViewBag.ChatSession = chatSession;

            TempData["Message"] = "Lịch sử chat đã xóa thành công.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ExportChat()
        {
            var exportContent = _chatbotApiService.ExportChatToText();
            var bytes = Encoding.UTF8.GetBytes(exportContent);
            var filename = $"English_Chat_Export_{DateTime.Now:ddMMyyyy_HHmmss}.txt";

            return File(bytes, "text/plain", filename);
        }

        [HttpPost]
        public IActionResult UpdateUserInfo(string username, string gender)
        {
            _chatbotApiService.UpdateUserInfo(username, gender);

            var chatSession = _chatbotApiService.GetChatSession();
            ViewBag.Username = username;
            ViewBag.Gender = gender;
            ViewBag.ChatSession = chatSession;

            return RedirectToAction("Index");
        }


    }
}
