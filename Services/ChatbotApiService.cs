using Microsoft.AspNetCore.Http;
using Mscc.GenerativeAI;
using Project1.Models.ViewModels;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class ChatbotApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string SessionKey = "ChatSession";
    private readonly ILogger<ChatbotApiService> _logger;

    public ChatbotApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ILogger<ChatbotApiService> logger)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;

        // Configure HttpClient
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<string> GenerateAnswerAsync(
        string apiKey,
        object requestPayload,
        string username,
        string gender,
        sbyte age,
        int englishLevel,
        bool enableReasoning,
        bool enableSearching)
    {
        try
        {
            var url = $"https://localhost:5000/api/Chatbot/GenerateAnswer?username={username}&gender={gender}&age={age}&englishLevel={englishLevel}&enableReasoning={enableReasoning}&enableSearching={enableSearching}";

            var jsonPayload = JsonSerializer.Serialize(requestPayload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            _logger.LogInformation($"Sending request to: {url}");
            _logger.LogInformation($"Payload: {jsonPayload}");

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };
            
            // Add API key to headers
            if (!string.IsNullOrEmpty(apiKey))
            {
                request.Headers.Add("Authentication", apiKey);
            }
            else
            {
                _logger.LogWarning("No API key provided for chatbot request");
            }

            var response = await _httpClient.SendAsync(request);
            _logger.LogInformation($"Response status: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Response received: {responseContent}");
                return responseContent;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"API request failed with status {response.StatusCode}: {errorContent}");
                return $"Tôi xin lỗi, nhưng dịch vụ đang gặp sự cố (HTTP {response.StatusCode}). Xin vui lòng thử lại sau.";
            }
        }
        catch (HttpRequestException httpEx)
        {
            _logger.LogError(httpEx, "HTTP request error to chatbot API");
            return $"Tôi xin lỗi, nhưng không thể kết nối đến dịch vụ chatbot. Lỗi: {httpEx.Message}";
        }
        catch (TaskCanceledException timeoutEx)
        {
            _logger.LogError(timeoutEx, "Chatbot API request timeout");
            return "Tôi xin lỗi, nhưng yêu cầu đã hết thời gian chờ. Vui lòng thử lại.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in GenerateAnswerAsync");
            return $"Tôi xin lỗi, nhưng đang có vấn đề xử lý yêu cầu của bạn. Lỗi: {ex.Message}";
        }
    }

    // Add the session management methods we discussed
    public Project1.Models.ViewModels.ChatSession GetChatSession()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session == null) return new Project1.Models.ViewModels.ChatSession();

        var chatSessionJson = session.GetString(SessionKey);
        return chatSessionJson != null ?
            JsonSerializer.Deserialize<Project1.Models.ViewModels.ChatSession>(chatSessionJson) ?? new Project1.Models.ViewModels.ChatSession() :
            new Project1.Models.ViewModels.ChatSession();
    }

    public void SaveChatSession(Project1.Models.ViewModels.ChatSession chatSession)
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session != null)
        {
            session.SetString(SessionKey, JsonSerializer.Serialize(chatSession));
        }
    }

    public void AddUserMessage(string username, string message)
    {
        var chatSession = GetChatSession();
        chatSession.Username = username;
        chatSession.AddUserMessage(message);
        SaveChatSession(chatSession);
    }

    public void AddBotMessage(string message)
    {
        var chatSession = GetChatSession();
        chatSession.AddBotMessage(message);
        SaveChatSession(chatSession);
    }

    public void UpdateUserInfo(string username, string gender)
    {
        var chatSession = GetChatSession();
        chatSession.Username = username;
        chatSession.Gender = gender;
        SaveChatSession(chatSession);
    }

    public void ClearChat()
    {
        var chatSession = GetChatSession();
        var username = chatSession.Username;
        var gender = chatSession.Gender;

        // Clear messages but keep user info
        chatSession.Messages.Clear();
        chatSession.Username = username;
        chatSession.Gender = gender;
        chatSession.LastActivity = DateTime.Now;

        SaveChatSession(chatSession);
    }

    public string ExportChatToText()
    {
        var chatSession = GetChatSession();
        var sb = new StringBuilder();

        sb.AppendLine("English Assistant Chatbot - Conversation Export");
        sb.AppendLine("==============================================");
        sb.AppendLine($"Export Date: {DateTime.Now:dd-MM-yyyy HH:mm:ss}");
        sb.AppendLine($"User: {chatSession.Username}");
        sb.AppendLine($"Gender: {chatSession.Gender}");
        sb.AppendLine($"Session Created: {chatSession.CreatedAt:dd-MM-yyyy HH:mm:ss}");
        sb.AppendLine($"Last Activity: {chatSession.LastActivity:dd-MM-yyyy HH:mm:ss}");
        sb.AppendLine();
        sb.AppendLine("Conversation History:");
        sb.AppendLine("-------------------");

        foreach (var message in chatSession.Messages)
        {
            var senderLabel = message.MessageType == "user" ? "User" : "Assistant";
            sb.AppendLine($"[{senderLabel} - {message.Timestamp:HH:mm}]");
            sb.AppendLine(message.Message);
            sb.AppendLine();
        }

        if (!chatSession.Messages.Any())
        {
            sb.AppendLine("No messages in this session.");
        }

        return sb.ToString();
    }
}