using Microsoft.AspNetCore.Http;
using System;

namespace chat.sssu.Models
{
    public class Message
    {
        public int Id { get; set; }
       
        public string ChatId { get; set; }

        public string SenderId { get; set; }

        public string SenderName { get; set; }

        public string Text { get; set; }

        public DateTime Date { get; set; }

        public string Timestamp { get; set; }

    }

    public class UploadedMessage
    {
        public int Id { get; set; }

        public string ChatId { get; set; }

        public string SenderId { get; set; }

        public string SenderName { get; set; }

        public string Text { get; set; }

        public string Timestamp { get; set; }

        public IFormFile File { get; set; }

    }

    public class PrepearedMessage
    {
        public string ChatId { get; set; }

        public string SenderId { get; set; }

        public string SenderName { get; set; }

        public string Text { get; set; }

        public File File { get; set; }

        public string Timestamp { get; set; }
     
    }
}
