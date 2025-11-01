using System;
using System.Collections.Generic;

namespace MeetingSummarize.Models
{
    public class Meeting
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public DateTimeOffset StartTime { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? EndTime { get; set; }
        public string? Transcript { get; set; }
        public string? Summary { get; set; }
        public string? Decisions { get; set; }
        public string? Actions { get; set; }

        public ICollection<Participant> Participants { get; set; } = new List<Participant>();
        public ICollection<ActionItem> ActionItems { get; set; } = new List<ActionItem>();
    }
}


