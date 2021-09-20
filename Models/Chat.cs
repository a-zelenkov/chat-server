using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace chat.sssu.Models
{
    public class Chat
    {
        public int Id { get; set; }

        public string Name { get; set; }

    }

    public class ChatConnection
    {
        public string ConnectionId { get; set; }

        public string ChatId { get; set; }

    }
}
