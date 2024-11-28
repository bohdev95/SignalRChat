using System;
using System.ComponentModel.DataAnnotations;

namespace groupChat.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string ConnectionID { get; set; }
        public DateTime Timestamp { get; set; }
    }
}