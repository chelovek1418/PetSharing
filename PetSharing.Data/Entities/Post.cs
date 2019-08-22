using System;
using System.Collections.Generic;
using System.Text;

namespace PetSharing.Data.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public int PetId { get; set; }
        public virtual PetProfile Pet { get; set; }
        public string Img { get; set; }
        public string Text { get; set; }
        public DateTime? Date { get; set; }
        public int LikeCount { get; set; }
        public bool IsDeleted { get; set; }

        public List<Comment> Comments { get; set; }
    }
}
