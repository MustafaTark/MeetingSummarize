using System;

namespace MeetingSummarize.Models
{
    public class Participant
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;

        public Guid MeetingId { get; set; }
        public Meeting? Meeting { get; set; }

        public DateTimeOffset JoinTime { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? LeaveTime { get; set; }
    }
}


