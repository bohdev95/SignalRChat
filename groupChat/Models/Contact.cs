using System;
using System.ComponentModel.DataAnnotations;

namespace groupChat.Models
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Friend { get; set; }

        public DateTime Timestamp { get; set; }
    }
}