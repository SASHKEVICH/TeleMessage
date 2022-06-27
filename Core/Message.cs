using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Core
{
    public class Message
    {
        public Guid MessageId { get; set; }
        public string Text { get; set; }
        public string SenderNickname { get; set; }
        public DateTime Time { get; set; }
    }
}
