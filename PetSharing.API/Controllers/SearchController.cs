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
        private IUserService _userService;
        private IService<PetProfileDto> _petProfileService;


        public SearchController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index(string name, SearchContract contract, int page=1)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (contract.IsPet == true)
                {
                    var petProfiles = await _petProfileService.GetAll((page - 1) * 20);
                    List<PetProfileMidInfoContract> pets = new List<PetProfileMidInfoContract>();
                    pets.AddRange(petProfiles.Where(x => x.Name.Contains(name)).Select(y=>new PetProfileMidInfoContract { Id = y.Id,
                    Breed = y.Breed,
                    Gender = y.Gender,
                    Img=y.Img,
                    IsReadyForSex=y.IsReadyForSex,
                    IsSale=y.IsSale,
                    IsShare=y.IsShare,
                    Name=y.Name,
                    Type=y.Type,
                    Location=y.Location}));
                    return Ok(pets);
                }
                else
                {
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
            return Ok(contract);
        }
    }
}