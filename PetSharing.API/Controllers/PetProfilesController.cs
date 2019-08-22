using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetSharing.API.Extensions;
using PetSharing.Contracts;
using PetSharing.Contracts.PetProfilesContract;
using PetSharing.Contracts.PostContracts;
using PetSharing.Contracts.UserContracts;
using PetSharing.Domain.Dtos;
using PetSharing.Domain.Models;
using PetSharing.Domain.Services;

namespace PetSharing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetProfilesController : ControllerBase
    {
        private IService<PetProfileDto> _petService;
        private IUserService _userService;

        public PetProfilesController(IService<PetProfileDto> petService, IUserService userService)
        {
            _petService = petService;
            _userService = userService;
        }

        public async Task<IActionResult> Subscribe(int id)
        {
            await _userService.Subscribe((await _userService.GetCurrentUserAsync(User)).Id, id);
            return RedirectToAction();
        }

        public async Task<IActionResult> UnSubscribe(int id)
        {
            await _userService.UnSubscribe((await _userService.GetCurrentUserAsync(User)).Id, id);
            return RedirectToAction();
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1) => Ok((await _petService.GetAll((page - 1) * 20))
            .Select(x => new PetProfileMidInfoContract
            {
                Id = x.Id,
                Name = x.Name,
                Img = x.Img,
                AvgLikeCount = x.AvgLikeCount,
                Breed = x.Breed,
                DateOfBirth = x.DateOfBirth,
                Gender = (Contracts.Genders)x.Gender,
                IsReadyForSex = x.IsReadyForSex,
                IsSale = x.IsSale,
                IsShare = x.IsShare,
                Location = x.Location,
                Type = x.Type
            }).ToList());

        [HttpGet]
        [Route("[action]/{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserProfiles(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            return Ok((await _userService.GetPetProfiles(id))
            .Select(x => new PetProfileMidInfoContract
            {
                Id = x.Id,
                Name = x.Name,
                Img = x.Img,
                AvgLikeCount = x.AvgLikeCount,
                Breed = x.Breed,
                DateOfBirth = x.DateOfBirth,
                Gender = (Contracts.Genders)x.Gender,
                IsReadyForSex = x.IsReadyForSex,
                IsSale = x.IsSale,
                IsShare = x.IsShare,
                Location = x.Location,
                Type = x.Type
            }).ToList());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var petProfile = await _petService.GetById(id);
                var petProfileView = petProfile.ToContract();
                var owner = _userService.FindById(petProfile.OwnerId);
                petProfileView.Owner = new UserShortInfoContract
                {
                    Id = owner.Result.Id,
                    UserName = owner.Result.UserName,
                    PicUrl = owner.Result.PicUrl
                };
                petProfileView.Posts = petProfile.Posts.Select(x => new PostShortContract
                {
                    Id = x.Id,
                    Text = x.Text,
                    Img = x.Img,
                    Date = x.Date,
                    LikeCount = x.LikeCount
                }).ToList();
                return Ok(petProfileView);
            }
            catch
            {
                return BadRequest();
            }
        }

        [Authorize]
        public IActionResult Create() => Ok(new List<Contracts.Genders> { Contracts.Genders.Male, Contracts.Genders.Female, Contracts.Genders.Hermaphrodite });

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(PetProfileCreateContract pet)
        {
            if (ModelState.IsValid)
            {
                var petDto = new PetProfileDto
                {
                    OwnerId = (await _userService.GetCurrentUserAsync(User)).Id,
                    Breed = pet.Breed,
                    DateOfBirth = pet.DateOfBirth,
                    Gender = (Domain.Models.Genders)pet.Gender,
                    Img = pet.Img,
                    Type = pet.Type,
                    Name = pet.Name,
                    Location = pet.Location,
                    IsSale = pet.IsSale,
                    IsShare = pet.IsShare,
                    IsReadyForSex = pet.IsReadyForSex
                };
                int id = await _petService.Create(petDto);
                return RedirectToAction("GetById", "PetProfiles", $"{id}");
            }
            return Ok(pet);
        }

        [Authorize]
        public async Task<IActionResult> EditPetProfile(int id)
        {
            var pet = await _petService.GetById(id);
            if (pet != null && (await _userService.GetCurrentUserAsync(User)).Id == pet.OwnerId)
                return Ok(new PetProfileEditContract
                {
                    Breed = pet.Breed,
                    DateOfBirth = pet.DateOfBirth,
                    Gender = (Contracts.Genders)pet.Gender,
                    Img = pet.Img,
                    IsReadyForSex = pet.IsReadyForSex,
                    IsSale = pet.IsSale,
                    IsShare = pet.IsShare,
                    Location = pet.Location,
                    Name = pet.Name,
                    Type = pet.Type,
                    Id = pet.Id
                });
            return BadRequest();
        }

        [HttpPut]
        public async Task<IActionResult> EditPetProfile(PetProfileEditContract model)
        {
            if (ModelState.IsValid)
            {
                var pet = await _petService.GetById(model.Id);
                if (pet != null && (await _userService.GetCurrentUserAsync(User)).Id == pet.OwnerId)
                {
                    pet.Breed = model.Breed;
                    pet.DateOfBirth = model.DateOfBirth;
                    pet.Gender = (Domain.Models.Genders)model.Gender;
                    pet.Img = model.Img;
                    pet.IsReadyForSex = model.IsReadyForSex;
                    pet.IsSale = model.IsSale;
                    pet.IsShare = model.IsShare;
                    pet.Location = model.Location;
                    pet.Name = model.Name;
                    await _petService.Update(pet);
                    return RedirectToAction("GetById", "PetProfiles", $"{pet.Id}");
                }
            }
            return Ok(model);
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var pet = await _petService.GetById(id);
            if (pet == null || (await _userService.GetCurrentUserAsync(User)).Id != pet.OwnerId)
                return BadRequest();
            await _petService.Delete(id);
            return RedirectToAction("Index", "Home");
        }
    }
}