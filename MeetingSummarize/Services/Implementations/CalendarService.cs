using System;
using System.Threading.Tasks;
using MeetingSummarize.Services.Interfaces;

namespace MeetingSummarize.Services.Implementations
{
    public class CalendarService : ICalendarService
    {
        public Task ScheduleActionAsync(string title, string assigneeEmail, DateTimeOffset dueDate)
        {
            // Mocked implementation for now
            return Task.CompletedTask;
        }
    }
}


