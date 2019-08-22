using PetSharing.Data.Entities;
using PetSharing.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetSharing.Domain.Extensions
{
    public static class CommentExtensions
    {
        public static Comment ToEntity(this CommentDto dto)
        {
            if (dto == null)
                return null;
            return new Comment()
            {
                Id = dto.Id,
                PostId = dto.PostId,
                UserId = dto.UserId,
                Date = dto.Date,
                Text = dto.Text,
                LikeCount = dto.LikeCount
            };
        }
        public static CommentDto ToDto(this Comment entity)
        {
            if (entity == null)
                return null;
            return new CommentDto()
            {
                Id = entity.Id,
                Date = entity.Date,
                PostId = entity.PostId,
                UserId = entity.UserId,
                Text = entity.Text,
                LikeCount = entity.LikeCount
            };
        }
    }
}
