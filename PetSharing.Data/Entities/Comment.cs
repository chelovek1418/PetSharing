using System;
using System.Collections.Generic;
using System.Text;

namespace PetSharing.Data.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int LikeCount { get; set; }
        public string Text { get; set; }
        public DateTime? Date { get; set; }

        public bool IsDeleted { get; set; }

        public virtual User User { get; set; }
        public string UserId { get; set; }
    }
}
