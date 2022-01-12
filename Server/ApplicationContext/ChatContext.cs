using Core;
using Microsoft.EntityFrameworkCore;

namespace Server.ApplicationContext
{
    public class ChatContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }

        public ChatContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-UM3AQJP;Database=messagesDB;Trusted_Connection=True;");
        }
    }
}