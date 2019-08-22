using PetSharing.Contracts;
using PetSharing.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetSharing.API.Extensions
{
    public static class ChatExtensions
    {
        public static ChatContract ToContract(this ChatDto dto)
        {
            if (dto == null)
                return null;
            return new ChatContract()
            {
                Date = dto.Date,
                Id = dto.Id,
                Name = dto.Name,
                LastMessage = dto.LastMessage,
                PicUrl = dto.Pic
            };
        }
    }
}
