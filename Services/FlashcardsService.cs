using Microsoft.EntityFrameworkCore;
using Project1.Models;
using Project1.Models.ViewModels;
using Project1.Models.ViewModels;
using System.Net.Http;

namespace Project1.Services
{
    public class FlashcardsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public FlashcardsService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiBaseUrl"] ?? "https://localhost:7290/api"; // Configure in appsettings.json
            _httpClient.BaseAddress = new Uri(_baseApiUrl);
        }

        public async Task<List<TuVungVM>> GetAllUserFlashcardsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<TuVungVM>>>("/api/FlashcardsAPI/all");
            return response?.Data ?? new List<TuVungVM>();
        }

        public async Task<ApiPagedResponse<TuVungVM>> GetUserFlashcardsPagedAsync(int page, int pageSize)
        {
            var url = $"/api/FlashcardsAPI?page={page}&pageSize={pageSize}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ApiPagedResponse<TuVungVM>>>();
                return apiResponse?.Data;
            }
            return null;
        }

        public async Task<TuVungVM?> GetFlashcardAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/FlashcardsAPI/{id}");
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<TuVungVM>>();
                return apiResponse?.Data;
            }
            return null;
        }

        public async Task<TuVungVM?> AddFlashcardAsync(int tuVungId)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/FlashcardsAPI", tuVungId);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<TuVungVM>>();
                return apiResponse?.Data;
            }
            return null;
        }

        public async Task<bool> RemoveFlashcardAsync(int tuVungId)
        {
            var response = await _httpClient.DeleteAsync($"/api/FlashcardsAPI/{tuVungId}");
            return response.IsSuccessStatusCode;
        }
    }
}
