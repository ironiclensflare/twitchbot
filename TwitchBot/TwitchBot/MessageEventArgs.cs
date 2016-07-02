using System;

namespace TwitchBot
{
    public class MessageEventArgs
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsMod { get; set; }

        public MessageEventArgs(string name, string message)
        {
            Name = name;
            Message = message;
            Timestamp = DateTime.Now;
        }
    }
}
