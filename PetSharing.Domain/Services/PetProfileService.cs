using PetSharing.Data.Contexts;
using PetSharing.Data.Entities;
using PetSharing.Data.UnitOfWorks;
using PetSharing.Domain.Dtos;
using PetSharing.Domain.Extensions;
using PetSharing.Domain.Infrastructure;
using PetSharing.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetSharing.Domain.Services
{
    public class PetProfileService : IService<PetProfileDto>
    {
        private IUnitOfWork _unitOfWork { get; set; }

        public PetProfileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PetProfileDto>> GetBySub(int skip, string id)
        {
            return (await _unitOfWork.PetProfiles.GetBySub(skip, id)).Select(x => x.ToDto());
        }

        public async Task<IEnumerable<PetProfileDto>> GetAll(int skip)
        {
            return (await _unitOfWork.PetProfiles.GetAllAsync(skip)).Select(x => x.ToDto());
        }

        public async Task<PetProfileDto> GetById(int id)
        {
            var profile = await _unitOfWork.PetProfiles.GetAsync(id);
            if (profile == null)
                throw new ValidationException("Профиль не найден", "");
            var profileDto = profile.ToDto();
            profileDto.Posts = profile.Posts.Select(x => x.ToDto()).OrderByDescending(y => y.Date).ToList();
            return profileDto;
        }

        public async Task<int> Create(PetProfileDto petDto)
        {
            return await _unitOfWork.PetProfiles.CreateAsync(petDto.ToEntity());
        }

        public async Task Update(PetProfileDto petDto)
        {
            var pet = await _unitOfWork.PetProfiles.GetAsync(petDto.Id);
            pet.Breed = petDto.Breed;
            pet.DateOfBirth = petDto.DateOfBirth;
            pet.Gender = petDto.Gender == null ? null : (Genders?)Enum.Parse(typeof(Genders), petDto.Gender);
            pet.Img = petDto.Img;
            pet.IsReadyForSex = petDto.IsReadyForSex;
            pet.IsSale = petDto.IsSale;
            pet.IsShare = petDto.IsShare;
            pet.Location = petDto.Location;
            pet.Name = petDto.Name;
            await _unitOfWork.PetProfiles.UpdateAsync(pet);
        }

        public async Task Delete(int id)
        {
            await _unitOfWork.PetProfiles.DeleteAsync(id);
        }
    }
}
