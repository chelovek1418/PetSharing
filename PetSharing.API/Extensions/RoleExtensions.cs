using PetSharing.Contracts.AdminConracts;
using PetSharing.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetSharing.API.Extensions
{
    public static class RoleExtensions
    {
        public static RoleContract ToContract(this RoleDto dto)
        {
            if (dto == null)
                return null;
            return new RoleContract()
            {
                Id = dto.Id,
                Name = dto.Name
            };
        }
        public static RoleDto ToDto(this RoleContract contract)
        {
            if (contract == null)
                return null;
            return new RoleDto()
            {
                Id = contract.Id,
                Name = contract.Name
            };
        }
    }
}
