using PetSharing.Data.Entities;
using PetSharing.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetSharing.Domain.Extensions
{
    public static class MessageExtensions
    {
        public static Message ToEntity(this MessageDto dto)
        {
            if (dto == null)
                return null;
            return new Message()
            {
                UserId=dto.UserId,
                Id = dto.Id,
                ReceiverId = dto.ReceiverId,
                Date = dto.Date,
                Text = dto.Text
            };
        }
        public static MessageDto ToDto(this Message entity)
        {
            if (entity == null)
                return null;
            return new MessageDto()
            {
                UserId = entity.UserId,
                Id = entity.Id,
                Date = entity.Date,
                Text = entity.Text,
                ReceiverId = entity.ReceiverId
            };
        }
    }
}
