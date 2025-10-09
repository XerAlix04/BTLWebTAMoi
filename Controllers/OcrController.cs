using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Project1.Controllers
{
    public class OcrController : Controller
    {
        private readonly HttpClient _httpClient;

        public OcrController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost]
        public async Task<IActionResult> Recognize(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "Không có file nào được chọn." });

            using var content = new MultipartFormDataContent();
            using var stream = file.OpenReadStream();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "file", file.FileName);

            // Gọi đến Python FastAPI
            var response = await _httpClient.PostAsync("http://127.0.0.1:8000/recognize", content);

            var result = await response.Content.ReadAsStringAsync();
            return Content(result, "application/json");
        }
    }
}
