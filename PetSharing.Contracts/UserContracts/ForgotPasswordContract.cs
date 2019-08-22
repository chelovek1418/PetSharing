using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetSharing.Contracts
{
    public class ForgotPasswordContract
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
