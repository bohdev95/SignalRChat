using System.Data.Entity;
using groupChat.Models;

namespace groupChat.Data
{
    public class ChatInitializer : CreateDatabaseIfNotExists<ChatContext>
    {
        protected override void Seed(ChatContext context)
        {
            context.Users.Add(new User
            {
                Username = "admin",
                ConnectionID = "admin",
                Timestamp = System.DateTime.Now
            });

            context.SaveChanges();
        }
    }
}