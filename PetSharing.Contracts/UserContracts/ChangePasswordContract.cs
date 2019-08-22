using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetSharing.Contracts
{
    public class ChangePasswordContract
    {
        public string Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string ConfirnNewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
    }
}
