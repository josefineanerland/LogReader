using System;

namespace Reader
{
    public class LogModel
    {
        public TimeSpan timeStamp { get; set; }
        public int sessionId { get; set; }
        public string Event { get; set; }
        public string Data { get; set; }

    }
}
