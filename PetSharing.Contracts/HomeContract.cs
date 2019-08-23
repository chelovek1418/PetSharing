using PetSharing.Contracts.PetProfilesContract;
using PetSharing.Contracts.UserContracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetSharing.Contracts
{
    public class HomeContract
    {
        //public string Phone { get; set; }
        //public string FullName { get; set; }
        //public string Email { get; set; }
        //public string PicUrl { get; set; }
        //public string UserName { get; set; }
        public UserFullInfoContract User { get; set; }

        public List<PetProfileShortInfoContract> Pets { get; set; }
        public List<PostContract> Posts { get; set; }

        public HomeContract()
        {
            Pets = new List<PetProfileShortInfoContract>();
            Posts = new List<PostContract>();
        }
    }
}
