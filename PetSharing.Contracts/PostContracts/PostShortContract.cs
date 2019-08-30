using PetSharing.Contracts.PetProfilesContract;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetSharing.Contracts.PostContracts
{
    public class PostShortContract
    {
        public int Id { get; set; }
        public string Img { get; set; }
        public string Text { get; set; }
        public DateTime? Date { get; set; }
        public int LikeCount { get; set; }
        public PetProfileShortInfoContract Pet { get; set; }

        public PostShortContract()
        {
            Pet = new PetProfileShortInfoContract();
        }
    }
}
