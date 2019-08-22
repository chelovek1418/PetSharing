using PetSharing.Contracts.PetProfilesContract;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetSharing.Contracts
{
    public class PostContract
    {
        //public int PetId { get; set; }
        public int Id { get; set; }
        public string Img { get; set; }
        public string Text { get; set; }
        public DateTime? Date { get; set; }
        public int LikeCount { get; set; }
        public List<CommentContract> Comments { get; set; }
        public PetProfileShortInfoContract Pet { get; set; }

        public PostContract()
        {
            Pet = new PetProfileShortInfoContract();
            Comments = new List<CommentContract>();
        }
    }
}
