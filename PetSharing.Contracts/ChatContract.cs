using System;
using System.Collections.Generic;
using System.Text;

namespace PetSharing.Contracts
{
    public class ChatContract
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PicUrl { get; set; }
        public DateTime? Date { get; set; }
        public string LastMessage { get; set; }
        public List<MessageContract> Messages { get; set; }
        public ChatContract()
        {
            Messages = new List<MessageContract>();
        }
    }
}
