using PetSharing.Data.Entities;
using PetSharing.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetSharing.Domain.Extensions
{
    public static class UserExtensions
    {
        public static User ToEntity(this UserDto dto)
        {
            if (dto == null)
                return null;
            return new User()
            {
                Id = dto.Id,
                UserName = dto.UserName,
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.Phone,
                PicUrl = dto.PicUrl
            };
        }
        public static UserDto ToDto(this User user)
        {
            if (user == null)
                return null;
            return new UserDto()
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.PhoneNumber,
                PicUrl = user.PicUrl,
                UserName = user.UserName
            };
        }
    }
}