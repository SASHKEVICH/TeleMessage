namespace Server.ApplicationContext
{
    public class ChatContextSeeder
    {
        public static void Seed(ChatContext context)
        {
            context.Database.EnsureCreated();
            context.SaveChanges();
        }
    }
}

