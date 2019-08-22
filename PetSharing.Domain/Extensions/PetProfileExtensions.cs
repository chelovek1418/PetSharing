using PetSharing.Data.Entities;
using PetSharing.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetSharing.Domain.Extensions
{
    public static class PetProfileExtensions
    {
        public static PetProfile ToEntity(this PetProfileDto dto)
        {
            if (dto == null)
                return null;
            return new PetProfile
            {
                Id = dto.Id,
                OwnerId = dto.OwnerId,
                AvgLikeCount = dto.AvgLikeCount,
                Breed = dto.Breed,
                DateOfBirth = dto.DateOfBirth,
                Gender = (Data.Entities.Genders)dto.Gender,
                Img = dto.Img,
                IsReadyForSex = dto.IsReadyForSex,
                IsSale = dto.IsSale,
                IsShare = dto.IsShare,
                Location = dto.Location,
                Name = dto.Name,
                Type = dto.Type
            };
        }
        public static PetProfileDto ToDto(this PetProfile pet)
        {
            if (pet == null)
                return null;
            return new PetProfileDto()
            {
                Id = pet.Id,
                OwnerId = pet.OwnerId,
                AvgLikeCount = pet.AvgLikeCount,
                Breed = pet.Breed,
                DateOfBirth = pet.DateOfBirth,
                Gender = (Models.Genders)pet.Gender,
                Img = pet.Img,
                IsReadyForSex = pet.IsReadyForSex,
                IsSale = pet.IsSale,
                IsShare = pet.IsShare,
                Location = pet.Location,
                Name = pet.Name,
                Type = pet.Type
            };
        }
    }
}
