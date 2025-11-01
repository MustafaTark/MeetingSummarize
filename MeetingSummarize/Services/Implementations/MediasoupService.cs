using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MeetingSummarize.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MeetingSummarize.Services.Implementations
{
    public class MediasoupService : IMediasoupService
    {
        private readonly HttpClient _httpClient;
        private readonly string _serverUrl;

        public MediasoupService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _serverUrl = configuration["Mediasoup:ServerUrl"] ?? "http://localhost:3001";
        }

        public Task StartRoomAsync(string meetingId)
        {
            return PostAsync("/rooms/start", new { meetingId });
        }

        public Task AddPeerAsync(string meetingId, string connectionId)
        {
            return PostAsync("/rooms/addPeer", new { meetingId, connectionId });
        }

        public Task CloseRoomAsync(string meetingId)
        {
            return PostAsync("/rooms/close", new { meetingId });
        }

        private async Task PostAsync(string path, object payload)
        {
            var json = JsonSerializer.Serialize(payload);
            var res = await _httpClient.PostAsync(_serverUrl + path, new StringContent(json, Encoding.UTF8, "application/json"));
            res.EnsureSuccessStatusCode();
        }
    }
}


