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
            if (profile != null)
            {
                var profileDto = profile.ToDto();
                profileDto.Posts = profile.Posts.Select(x=>x.ToDto()).OrderByDescending(y=>y.Date).ToList();
                return profileDto;
            }
            throw new ValidationException("Профиль не найден", "");
        }

        public async Task<int> Create(PetProfileDto petDto)
        {
            return await _unitOfWork.PetProfiles.CreateAsync(petDto.ToEntity());
        }

        public async Task Update(PetProfileDto petDto)
        {
            await _unitOfWork.PetProfiles.UpdateAsync(petDto.ToEntity());
        }

        public async Task Delete(int id)
        {
            await _unitOfWork.PetProfiles.DeleteAsync(id);
        }
    }
}
