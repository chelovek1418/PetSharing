using PetSharing.Contracts;
using PetSharing.Domain.Models;

namespace PetSharing.API.Extensions
{
    public static class UserExtensions
    {
        public static UserContract ToContract(this UserDto dto)
        {
            if ( dto == null)
                return null;
            return new UserContract
            {
                Id = dto.Id,
                Role = dto.Role,
                Email = dto.Email,
                FullName = dto.FullName,
                Password = dto.Password,
                Phone = dto.Phone,
                PicUrl = dto.PicUrl,
                UserName = dto.UserName
            };
        }
        public static UserDto ToDto(this UserContract contract)
        {
            if (contract == null)
                return null;
            return new UserDto
            {
                Id = contract.Id,
                Role = contract.Role,
                Email = contract.Email,
                FullName = contract.FullName,
                Password = contract.Password,
                Phone = contract.Phone,
                PicUrl = contract.PicUrl,
                UserName = contract.UserName
            };
        }
    }
}
