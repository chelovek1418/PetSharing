using PetSharing.Contracts.UserContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetSharing.Contracts.AdminConracts
{
    public class RoleContract
    {
        [Required]
        public string Name { get; set; }
        public string Id { get; set; }
    }
}
