using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetSharing.Contracts.PetProfilesContract
{
    public class PetProfileEditContract
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [DataType(DataType.ImageUrl)]
        public string Img { get; set; }
        public string Type { get; set; }
        public string Breed { get; set; }
        public Genders Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Location { get; set; }
        public bool IsSale { get; set; }
        public bool IsReadyForSex { get; set; }
        public bool IsShare { get; set; }

        public List<Genders> Genders { get; set; }

        public PetProfileEditContract()
        {
            Genders = new List<Genders> { Contracts.Genders.Male, Contracts.Genders.Female, Contracts.Genders.Hermaphrodite };
        }
    }
}
