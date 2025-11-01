using System.IO;
using System.Threading.Tasks;

namespace MeetingSummarize.Services.Interfaces
{
    public interface ITranscriptionService
    {
        Task<string> TranscribeChunkAsync(Stream audioStream, string fileName, string contentType);
    }
}


