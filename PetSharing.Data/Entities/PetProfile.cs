using System;
using System.Collections.Generic;
using System.Text;

namespace PetSharing.Data.Entities
{
    public class PetProfile
    {
        public string OwnerId { get; set; }
        public virtual User Owner { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
        public string Type { get; set; }
        public string Breed { get; set; }
        public Genders? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Location { get; set; }
        public double AvgLikeCount { get; set; }
        public bool IsSale { get; set; }
        public bool IsReadyForSex { get; set; }
        public bool IsShare { get; set; }
        public bool IsDeleted { get; set; }

        public virtual List<Post> Posts { get; set; }
        public virtual List<Subscription> Folowers { get; set; }
    }
}
