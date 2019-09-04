using System;
using System.Collections.Generic;
using System.Text;

namespace PetSharing.Domain.Dtos
{
    public class MessageDto
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public string UserId { get; set; }
        public string ReceiverId { get; set; }
        public string Text { get; set; }
    }
}
