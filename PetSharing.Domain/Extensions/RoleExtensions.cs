using Microsoft.AspNetCore.Identity;
using PetSharing.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetSharing.Domain.Extensions
{
    public static class RoleExtensions
    {
        public static IdentityRole ToEntity(this RoleDto dto)
        {
            if (dto == null)
                return null;
            return new IdentityRole()
            {
                Id = dto.Id,
                Name = dto.Name
            };
        }
        public static RoleDto ToDto(this IdentityRole role)
        {
            if (role == null)
                return null;
            return new RoleDto()
            {
                Id = role.Id,
                Name = role.Name
            };
        }
    }
}
