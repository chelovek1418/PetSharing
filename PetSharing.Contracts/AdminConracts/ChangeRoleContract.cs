using Microsoft.AspNetCore.Identity;
using PetSharing.Contracts.AdminConracts;
using System.Collections.Generic;

namespace PetSharing.Contracts
{
    public class ChangeRoleContract
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string PicUrl { get; set; }
        public List<RoleContract> AllRoles { get; set; }
        public string Role { get; set; }
        public ChangeRoleContract()
        {
            AllRoles = new List<RoleContract>();
        }
    }
}
