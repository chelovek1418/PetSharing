using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetSharing.Contracts
{
    public class CreateCommentContract
    {
        public int PostId { get; set; }
        [Required]
        public string Text { get; set; }
    }
}
