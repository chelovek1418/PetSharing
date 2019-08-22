using PetSharing.Contracts.PetProfilesContract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetSharing.Contracts.PostContracts
{
    public class CreatePostContract
    {
        [DataType(DataType.ImageUrl)]
        public int PetId { get; set; }
        public string Img { get; set; }
        [Required]
        public string Text { get; set; }
    }
}
