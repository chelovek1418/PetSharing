using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetSharing.Contracts;
using PetSharing.Contracts.PetProfilesContract;
using PetSharing.Contracts.UserContracts;
using PetSharing.Domain.Models;
using PetSharing.Domain.Services;

namespace PetSharing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SearchController : ControllerBase
    {
        readonly IUserService _userService;
        readonly IService<PetProfileDto> _petProfileService;


        public SearchController(IUserService userService, IService<PetProfileDto> petProfileService)
        {
            _petProfileService = petProfileService;
            _userService = userService;
        }

        public async Task<IActionResult> Index(string name, SearchContract contract, int page = 1)
        {
            if (string.IsNullOrEmpty(name))
                return Ok(contract);
            if (contract.IsPet == true)
            {
                var petProfiles = await _petProfileService.GetAll((page - 1) * 20);
                var pets = new List<PetProfileMidInfoContract>();
                pets.AddRange(petProfiles.Where(x => x.Name.Contains(name)).Select(y => new PetProfileMidInfoContract
                {
                    Id = y.Id,
                    Breed = y.Breed,
                    Gender = y.Gender,
                    Img = y.Img,
                    IsReadyForSex = y.IsReadyForSex,
                    IsSale = y.IsSale,
                    IsShare = y.IsShare,
                    Name = y.Name,
                    Type = y.Type,
                    Location = y.Location
                }));
                return Ok(pets);
            }
            return Ok(_userService.FindUsers(name).Select(x => new UserFullInfoContract
            {
                Id = x.Id,
                FullName = x.FullName,
                Email = x.Email,
                Phone = x.Phone,
                PicUrl = x.PicUrl,
                UserName = x.UserName
            }).ToList());
        }
    }
}