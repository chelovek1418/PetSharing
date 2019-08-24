using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetSharing.Contracts.PetProfilesContract
{
    public class PetProfileCreateContract
    {
        [Required]
        public string Name { get; set; }
        [DataType(DataType.ImageUrl)]
        public string Img { get; set; }
        [Required]
        public string Type { get; set; }
        public string Breed { get; set; }
        public string Gender { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        public string Location { get; set; }
        public bool IsSale { get; set; }
        public bool IsReadyForSex { get; set; }
        public bool IsShare { get; set; }
    }
}
