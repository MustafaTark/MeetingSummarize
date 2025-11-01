using System.Threading.Tasks;

namespace MeetingSummarize.Services.Interfaces
{
    public interface ISummarizationService
    {
        Task<(string summary, string decisions, string actions)> SummarizeAsync(string transcript);
    }
}


