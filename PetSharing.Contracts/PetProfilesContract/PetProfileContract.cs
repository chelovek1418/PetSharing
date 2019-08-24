using PetSharing.Contracts.PostContracts;
using PetSharing.Contracts.UserContracts;
using System;
using System.Collections.Generic;

namespace PetSharing.Contracts
{
    public class PetProfileContract
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
        public string Type { get; set; }
        public string Breed { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Location { get; set; }
        public double AvgLikeCount { get; set; }
        public bool IsSale { get; set; }
        public bool IsReadyForSex { get; set; }
        public bool IsShare { get; set; }
        public List<PostShortContract> Posts { get; set; }
        public UserShortInfoContract Owner {get;set;}

        public PetProfileContract()
        {
            Posts = new List<PostShortContract>();
            Owner = new UserShortInfoContract();
        }
    }
}
