using System.Threading.Tasks;
using MeetingSummarize.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace MeetingSummarize.Hubs
{
    public class MeetingHub : Hub
    {
        private readonly IMediasoupService _mediasoupService;

        public MeetingHub(IMediasoupService mediasoupService)
        {
            _mediasoupService = mediasoupService;
        }

        public async Task JoinRoom(string meetingId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, meetingId);
            await _mediasoupService.AddPeerAsync(meetingId, Context.ConnectionId);
            await Clients.OthersInGroup(meetingId).SendAsync("PeerJoined", Context.ConnectionId);
        }

        public Task SendSignal(string meetingId, string type, string payload)
        {
            return Clients.OthersInGroup(meetingId).SendAsync("Signal", new { type, payload, from = Context.ConnectionId });
        }

        public Task SendChatMessage(string meetingId, string message)
        {
            return Clients.Group(meetingId).SendAsync("ChatMessage", new { from = Context.ConnectionId, message });
        }
    }
}


