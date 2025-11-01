using System;
using System.Threading.Tasks;

namespace MeetingSummarize.Services.Interfaces
{
    public interface ICalendarService
    {
        Task ScheduleActionAsync(string title, string assigneeEmail, DateTimeOffset dueDate);
    }
}


