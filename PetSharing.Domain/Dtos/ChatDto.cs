using System;
using System.Collections.Generic;
using System.Text;

namespace PetSharing.Domain.Dtos
{
    public class ChatDto
    {
        public string Pic { get; set; }
        public string Id { get; set; }
        public DateTime? Date { get; set; }
        public string Name { get; set; }
        public string LastMessage { get; set; }
    }
}
