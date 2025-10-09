namespace Project1.Models.ViewModels
{
    public class ChatMessage
    {
        public string Sender { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string MessageType { get; set; } = "user"; // "user" or "bot"
    }

    public class ChatSession
    {
        public string Username { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime LastActivity { get; set; } = DateTime.Now;

        public void AddUserMessage(string message)
        {
            Messages.Add(new ChatMessage
            {
                Sender = Username,
                Message = message,
                Timestamp = DateTime.Now,
                MessageType = "user"
            });
            LastActivity = DateTime.Now;
        }

        public void AddBotMessage(string message)
        {
            Messages.Add(new ChatMessage
            {
                Sender = "English Assistant",
                Message = message,
                Timestamp = DateTime.Now,
                MessageType = "bot"
            });
            LastActivity = DateTime.Now;
        }
    }
}
