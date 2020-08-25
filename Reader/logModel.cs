using System;

namespace Reader
{
    public class LogModel
    {
        public TimeSpan TimeStamp { get; set; }
        public int SessionId { get; set; }
        public string Event { get; set; }
        public string Data { get; set; }

    }
}
