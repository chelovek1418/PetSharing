using System;
using System.Collections.Generic;
using System.Text;

namespace PetSharing.Contracts.UserContracts
{
    public class UserFullInfoContract
    {
        public string Id { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PicUrl { get; set; }
        public string UserName { get; set; }
    }
}
