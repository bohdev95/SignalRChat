using System;
using System.ComponentModel.DataAnnotations;

namespace groupChat.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Sender { get; set; }

        [Required]
        public string Receiver { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }
    }
}