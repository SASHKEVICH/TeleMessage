namespace Core
{
    public class ServerMessage
    {
        public List<Message>? InitialMessages { get; set; }
        public List<User>? ConnectedUsers { get; set; }
        public Message? Message { get; set; }
        public MessageType Type { get; set; }
    }
}

