using PetSharing.Data.Entities;
using PetSharing.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetSharing.Domain.Extensions
{
    public static class PostExtensions
    {
        public static Post ToEntity(this PostDto dto)
        {
            if (dto == null)
                return null;
            return new Post()
            {
                Id = dto.Id,
                PetId = dto.PetId,
                Date = dto.Date,
                Img = dto.Img,
                Text = dto.Text,
                LikeCount = dto.LikeCount
            };
        }
        public static PostDto ToDto(this Post post)
        {
            if (post == null)
                return null;
            return new PostDto()
            {
                Id = post.Id,
                PetId = post.PetId,
                Date = post.Date,
                Img = post.Img,
                Text = post.Text,
                LikeCount = post.LikeCount
            };
        }
    }
}
