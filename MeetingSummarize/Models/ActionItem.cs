using System;

namespace MeetingSummarize.Models
{
    public class ActionItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Description { get; set; } = string.Empty;
        public string? Assignee { get; set; }
        public DateTimeOffset? DueDate { get; set; }

        public Guid MeetingId { get; set; }
        public Meeting? Meeting { get; set; }
    }
}


