using System;
using System.Collections.Generic;
using System.Text;

namespace PetSharing.Contracts.PetProfilesContract
{
    public class PetProfileShortInfoContract
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PicUrl { get; set; }
    }
}
