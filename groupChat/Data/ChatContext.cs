using System.Data.Entity;
using groupChat.Models;

namespace groupChat.Data
{
    public class ChatContext : DbContext
    {
        public ChatContext() : base("ChatDbConnection") { }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Contact> Contacts { get; set; }
    }
}