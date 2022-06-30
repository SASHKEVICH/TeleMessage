using Core;
using Microsoft.EntityFrameworkCore;

namespace Server.ApplicationContext
{
    public class ChatContext : DbContext
    {
        #region Properties
        
        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }

        #endregion

        #region Constructor

        public ChatContext(DbContextOptions<ChatContext> options)
            : base(options)
        {
        }

        #endregion
        
    }
}