using System;
using System.Collections.Generic;
using System.Text;

namespace PetSharing.Domain.Models
{
    public class UserDto
    {
        public string Id { get; set; }
        public string PicUrl { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public List<PetProfileDto> PetProfiles { get; set; }

        public UserDto()
        {
            PetProfiles = new List<PetProfileDto>();
        }
    }
}
