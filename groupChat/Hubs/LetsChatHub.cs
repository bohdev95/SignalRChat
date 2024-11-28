using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using groupChat.Data;
using groupChat.Models;
using System;

namespace groupChat.Hubs
{

    public class ChatMessage
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public string FriendUniqueId { get; set; }
    }
    [HubName("letsChatHub")]
    public class LetsChatHub : Hub
    {
        private readonly ChatContext _context = new ChatContext();

        public async Task sendChatMessage(ChatMessage message)
        {
            var newMessage = new Message
            {
                Sender = message.Name,
                Receiver = message.FriendUniqueId,
                Content = message.Message,
                Timestamp = System.DateTime.Now
            };

            _context.Messages.Add(newMessage);

            await _context.SaveChangesAsync();

            if (message.FriendUniqueId == "All users") {

                Clients.AllExcept(Context.ConnectionId).addChatMessage(message.Name, message.FriendUniqueId, message.Message);
            } else
            {
                var receiver = _context.Users.FirstOrDefault(u => u.Username == message.FriendUniqueId);

                if (receiver != null)
                {
                    Clients.Client(receiver.ConnectionID).addChatMessage(message.Name, message.FriendUniqueId, message.Message);

                    var user = _context.Contacts.FirstOrDefault(u => u.Username == message.Name && u.Friend == message.FriendUniqueId);

                    if (user == null)
                    {
                        // Add the new user if it doesn't exist
                        var contact = new Contact
                        {
                            Username = message.Name,
                            Friend = message.FriendUniqueId,
                            Timestamp = DateTime.Now
                        };
                        _context.Contacts.Add(contact);
                        _context.SaveChanges();
                    }

                    // Persist the changes to the database
                    
                }
                
            }
           
        }

    }

}
