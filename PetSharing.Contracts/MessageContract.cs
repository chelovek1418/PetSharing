using System;
using System.Collections.Generic;
using System.Text;

namespace PetSharing.Contracts
{
    public class MessageContract
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime? Date { get; set; }
        public string ReceiverId { get; set; }
    }
}
