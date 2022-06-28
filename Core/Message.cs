using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Core
{
    public class Message
    {
        public Guid MessageId { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
        
        public Guid UserId { get; set; }
    }
}
