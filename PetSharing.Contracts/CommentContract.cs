using PetSharing.Contracts.UserContracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetSharing.Contracts
{
    public class CommentContract
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int LikeCount { get; set; }
        public string Text { get; set; }
        public DateTime? Date { get; set; }
        public UserShortInfoContract User {get;set;}

        public CommentContract()
        {
            if(User == null)
                User = new UserShortInfoContract();
        }
    }
}