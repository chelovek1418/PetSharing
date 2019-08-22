using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetSharing.Contracts
{
    public class EditUserContract
    {
        public string Id { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        [Phone]
        public string Phone { get; set; }
        [DataType(DataType.ImageUrl)]
        public string PicUrl { get; set; }
    }
}
