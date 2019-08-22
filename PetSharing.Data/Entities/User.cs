using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace PetSharing.Data.Entities
{
    public class User : IdentityUser
    {     
        public string PicUrl { get; set; }
        public string FullName { get; set; }
        public bool IsDeleted { get; set; }
        public virtual List<Subscription> Subscriptions { get; set; }
        public virtual List<Message> Messages { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public virtual List<PetProfile> PetProfiles { get; set; }
    }
}
