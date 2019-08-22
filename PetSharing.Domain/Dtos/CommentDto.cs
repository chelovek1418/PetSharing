using System;
using System.Collections.Generic;
using System.Text;

namespace PetSharing.Domain.Dtos
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public string UserPic { get; set; }
        public string UserName { get; set; }
        public int LikeCount { get; set; }
        public string Text { get; set; }
        public DateTime? Date { get; set; }
    }
}
