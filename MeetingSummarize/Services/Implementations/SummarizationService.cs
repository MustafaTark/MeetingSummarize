using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MeetingSummarize.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MeetingSummarize.Services.Implementations
{
    public class SummarizationService : ISummarizationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public SummarizationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenAI:ApiKey"] ?? string.Empty;
        }

        public async Task<(string summary, string decisions, string actions)> SummarizeAsync(string transcript)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                return ("", "", ""); // mock when no key
            }

            var body = new
            {
                model = "gpt-4o-mini",
                messages = new object[]
                {
                    new { role = "system", content = "You are a meeting assistant. Summarize the meeting and extract decisions/actions." },
                    new { role = "user", content = transcript }
                }
            };

            var json = JsonSerializer.Serialize(body);
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            // For brevity, return raw parts; parsing can be added later
            return (responseJson, responseJson, responseJson);
        }
    }
}


