using PetSharing.Contracts;
using PetSharing.Domain.Models;

namespace PetSharing.API.Extensions
{
    public static class PetProfileExtensions
    {
        public static PetProfileContract ToContract(this PetProfileDto dto)
        {
            if (dto == null)
                return null;
            return new PetProfileContract
            {
                AvgLikeCount = dto.AvgLikeCount,
                Breed = dto.Breed,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                Img = dto.Img,
                IsReadyForSex = dto.IsReadyForSex,
                IsSale = dto.IsSale,
                IsShare = dto.IsShare,
                Location = dto.Location,
                Name = dto.Name,
                Type = dto.Type,
                Id = dto.Id
            };
        }
        public static PetProfileDto ToDto(this PetProfileContract pet)
        {
            if (pet == null)
                return null;
            return new PetProfileDto()
            {
                AvgLikeCount = pet.AvgLikeCount,
                Breed = pet.Breed,
                DateOfBirth = pet.DateOfBirth,
                Gender = pet.Gender,
                Img = pet.Img,
                IsReadyForSex = pet.IsReadyForSex,
                IsSale = pet.IsSale,
                IsShare = pet.IsShare,
                Location = pet.Location,
                Name = pet.Name,
                Type = pet.Type,
                Id = pet.Id
            };
        }
    }
}
