using PetSharing.Contracts;
using PetSharing.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetSharing.API.Extensions
{
    public static class MessageExtensions
    {
        public static MessageContract ToContract(this MessageDto dto)
        {
            if (dto == null)
                return null;
            return new MessageContract()
            {
                Date = dto.Date,
                Id = dto.Id,
                Text=dto.Text,
                ReceiverId=dto.ReceiverId
            };
        }

        public static MessageDto ToDto(this MessageContract contract)
        {
            if (contract == null)
                return null;
            return new MessageDto()
            {
                Date = contract.Date,
                Id = contract.Id,
                Text = contract.Text,
                ReceiverId = contract.ReceiverId
            };
        }
    }
}
