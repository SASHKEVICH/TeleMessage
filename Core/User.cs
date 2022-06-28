using System;

namespace Core
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Nickname { get; set; }

        public List<Message> Messages { get; } = new();
    }
}