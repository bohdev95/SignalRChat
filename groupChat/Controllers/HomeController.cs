using groupChat.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using groupChat.Data;
using System.Data.Entity;

namespace groupChat.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index(WebsiteVistorData vistorDataViewModel)
        {
            vistorDataViewModel = vistorDataViewModel.getClientIPData();
            Networking networking = new Networking();
            string MACAdress = networking.getPhysicalAddress();
            string clientMachineName = networking.getClientMachineName();
            //string getDeviceId = networking.getDeviceId();
            //string getCPUId = networking.getCPUId();
            //string getUUID = networking.getUUID();
            vistorDataViewModel.MacId = MACAdress;
            vistorDataViewModel.ClientMachineName = clientMachineName;
            return View(vistorDataViewModel);
        }
        [HttpPost]
		public JsonResult ajaxSenderUserName(Connection connection)
		{
			var userName = connection.UserName;
			var connectionID = connection.ConnectionID;

			if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(connectionID))
			{
				FormsAuthenticationTicket ticket =
							new FormsAuthenticationTicket(1, userName,
														  DateTime.Now, DateTime.Now.AddMinutes(20),
														  true, connectionID, FormsAuthentication.FormsCookiePath);
				string hashcookies = FormsAuthentication.Encrypt(ticket);
				HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hashcookies)
				{
					Expires = ticket.Expiration
				};

				Response.Cookies.Add(cookie);
				FormsAuthentication.SetAuthCookie(userName, false);

                using (ChatContext context = new ChatContext())
                {
                    var user = context.Users.FirstOrDefault(u => u.Username == connection.UserName);

                    if (user != null)
                    {
                        // Update the ConnectionID and timestamp if the user exists
                        user.ConnectionID = connectionID;
                        user.Timestamp = DateTime.Now; // Update timestamp for consistency
                    }
                    else
                    {
                        // Add the new user if it doesn't exist
                        var newUser = new User
                        {
                            Username = connection.UserName,
                            ConnectionID = connectionID,
                            Timestamp = DateTime.Now
                        };
                        context.Users.Add(newUser);
                    }

                    // Persist the changes to the database
                    context.SaveChanges();

                }

               
                return Json(new { IsValid = true });
			}
			else
			{
				return Json(new { IsValid = false , userName = userName, connectionID= connectionID });
			}
		}

        [HttpPost]
        public JsonResult searchUserName(Connection connection)
        {
            
            using (ChatContext context = new ChatContext())
            {
                var messages = context.Users
                .OrderByDescending(m => m.Timestamp)
                .Take(50)
                .OrderBy(m => m.Timestamp)
                .ToList();

                var users = context.Users
                   .Where(u => u.Username.Contains(connection.UserName))
                   .ToList();

                return Json(new { users = users, request = messages });

            }
        }

        [HttpPost]
        public JsonResult getContactList(Connection connection)
        {
            using (ChatContext context = new ChatContext())
            {
                var users = context.Contacts
                  .Where(u => u.Username == connection.UserName)
                  .ToList();

                return Json(new { users = users });

            }
           
        }

        [HttpPost]
        public JsonResult loadMessages(ReceiveData data)
        {
            using (ChatContext context = new ChatContext())
            {

                if (data.receiver == "All users")
                {
                    var messages = context.Messages
                        .Where(m => m.Receiver == data.receiver)
                        .OrderByDescending(m => m.Timestamp) // Sort by newest first
                        .Take(50) // Get the last 50 messages
                        .OrderBy(m => m.Timestamp) // Restore chronological order
                        .ToList();
                    return Json(new { messages = messages, sender = data.sender, receiver = data.receiver });
                } else {
                    var messages = context.Messages
                        .Where(m =>
                            (m.Sender == data.sender && m.Receiver == data.receiver) ||
                            (m.Sender == data.receiver && m.Receiver == data.sender))
                        .OrderByDescending(m => m.Timestamp) // Sort by newest first
                        .Take(50) // Get the last 50 messages
                        .OrderBy(m => m.Timestamp) // Restore chronological order
                        .ToList();

                    return Json(new { messages = messages, sender = data.sender, receiver = data.receiver });
                }
                

            }

            
        }
    }

	public class Connection
	{

		public string UserName { get; set; }
		public string ConnectionID { get; set; }

        public DateTime Timestamp { get; set; }
    }

    public class ReceiveData
    {

        public string sender { get; set; }
        public string receiver { get; set; }
        
    }
}