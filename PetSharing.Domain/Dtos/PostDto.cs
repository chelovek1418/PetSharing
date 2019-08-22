using System;
using System.Collections.Generic;
using System.Text;

namespace PetSharing.Domain.Dtos
{
    public class PostDto
    {
        public int Id { get; set; }
        public int PetId { get; set; }
        public string Img { get; set; }
        public string Text { get; set; }
        public DateTime? Date { get; set; }
        public int LikeCount { get; set; }

        public List<CommentDto> Comments { get; set; }

        public PostDto()
        {
            Comments = new List<CommentDto>();
        }
    }
}
