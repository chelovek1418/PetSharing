using System;
using System.Collections.Generic;
using System.Text;

namespace PetSharing.Contracts.PostContracts
{
    public class EditPostContract
    {
        public int Id { get; set; }
        public string Img { get; set; }
        public string Text { get; set; }
    }
}
