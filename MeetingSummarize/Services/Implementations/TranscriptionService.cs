using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MeetingSummarize.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MeetingSummarize.Services.Implementations
{
    public class TranscriptionService : ITranscriptionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public TranscriptionService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenAI:ApiKey"] ?? string.Empty;
        }

        public async Task<string> TranscribeChunkAsync(Stream audioStream, string fileName, string contentType)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                return ""; // mock when no key
            }

            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(audioStream);
            // بعض المتصفحات تُرسل Content-Type يحمل معاملات (مثلاً: "audio/webm;codecs=opus")
            // MediaTypeHeaderValue يتوقع نوعًا صالحًا فقط (type/subtype). ننظفه لأخذ الجزء الأساسي فقط.
            var sanitizedContentType = contentType;
            var semicolonIndex = sanitizedContentType?.IndexOf(';') ?? -1;
            if (!string.IsNullOrWhiteSpace(sanitizedContentType) && semicolonIndex > 0)
            {
                sanitizedContentType = sanitizedContentType!.Substring(0, semicolonIndex).Trim();
            }

            if (!string.IsNullOrWhiteSpace(sanitizedContentType))
            {
                if (MediaTypeHeaderValue.TryParse(sanitizedContentType, out var parsed))
                {
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(parsed.MediaType);
                }

            }
            content.Add(streamContent, "file", fileName);
            content.Add(new StringContent("whisper-1"), "model");

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/audio/transcriptions");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            request.Content = content;

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            // For brevity, return raw json or parse as needed
            return json;
        }
    }
}


