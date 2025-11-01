using System.Threading.Tasks;

namespace MeetingSummarize.Services.Interfaces
{
    public interface IMediasoupService
    {
        Task StartRoomAsync(string meetingId);
        Task AddPeerAsync(string meetingId, string connectionId);
        Task CloseRoomAsync(string meetingId);
    }
}


