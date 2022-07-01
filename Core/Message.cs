using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core
{
    public class Message
    {
        [Key]
        public Guid MessageId { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
        
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
