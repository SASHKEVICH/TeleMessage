using System;
using System.ComponentModel.DataAnnotations;

namespace Core
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }
        [Required]
        public string? Nickname { get; set; }
        
        public ICollection<Message>? Messages { get; set; }
    }
}