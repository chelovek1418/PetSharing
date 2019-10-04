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
    [Authorize]
    public class PetProfilesController : ControllerBase
    {
        readonly IService<PetProfileDto> _petService;
        readonly IUserService _userService;

        public PetProfilesController(IService<PetProfileDto> petService, IUserService userService)
        {
            _petService = petService;
            _userService = userService;
        }

        [Route("subscribe")]
        public async Task<IActionResult> Subscribe(int? id)
        {
            if (id == null)
                return BadRequest();
            await _userService.Subscribe(User.Claims.FirstOrDefault(x => x.Type == "UserID").Value, (int)id);
            return Ok();
        }

        [Route("unsubscribe")]
        public async Task<IActionResult> UnSubscribe(int? id)
        {
            if (id==null)
                return BadRequest();
            await _userService.UnSubscribe(User.Claims.FirstOrDefault(x => x.Type == "UserID").Value, (int)id);
            return Ok();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(int page = 1) => Ok((await _petService.GetAll((page - 1) * 20))
            .Select(x => new PetProfileMidInfoContract
            {
                Id = x.Id,
                Name = x.Name,
                Img = x.Img,
                AvgLikeCount = x.AvgLikeCount,
                Breed = x.Breed,
                DateOfBirth = x.DateOfBirth,
                Gender = x.Gender,
                IsReadyForSex = x.IsReadyForSex,
                IsSale = x.IsSale,
                IsShare = x.IsShare,
                Location = x.Location,
                Type = x.Type
            }).ToList());

        [Route("getall")]
        [Authorize]
        public async Task<IActionResult> GetUserProfiles(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();
            return Ok((await _userService.GetPetProfiles(id))
            .Select(x => new PetProfileMidInfoContract
            {
                Id = x.Id,
                Name = x.Name,
                Img = x.Img,
                AvgLikeCount = x.AvgLikeCount,
                Breed = x.Breed,
                DateOfBirth = x.DateOfBirth,
                Gender = x.Gender,
                IsReadyForSex = x.IsReadyForSex,
                IsSale = x.IsSale,
                IsShare = x.IsShare,
                Location = x.Location,
                Type = x.Type
            }).ToList());
        }

        [Route("get")]
        public async Task<IActionResult> GetById(int? id)
        {
            if (id==null)
                return BadRequest();
            var petProfile = await _petService.GetById((int)id);
            var petProfileView = petProfile.ToContract();
            var owner = _userService.FindById(petProfile.OwnerId);
            bool isOwner = User.Claims.FirstOrDefault(x => x.Type == "UserID").Value == petProfile.OwnerId;
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
            return Ok(new { petProfileView, isOwner });
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(PetProfileCreateContract pet)
        {
            return Ok(await _petService.Create(new PetProfileDto
            {
                OwnerId = User.Claims.FirstOrDefault(x => x.Type == "UserID").Value,
                Breed = pet.Breed,
                DateOfBirth = pet.DateOfBirth,
                Gender = pet.Gender,
                Img = pet.Img,
                Type = pet.Type,
                Name = pet.Name,
                Location = pet.Location,
                IsSale = pet.IsSale,
                IsShare = pet.IsShare,
                IsReadyForSex = pet.IsReadyForSex
            }));
        }

        [Route("edit")]
        public async Task<IActionResult> EditPetProfile(int? id)
        {
            if (id == null)
                return BadRequest();
            var pet = await _petService.GetById((int)id);
            if (pet == null || User.Claims.FirstOrDefault(x => x.Type == "UserID").Value != pet.OwnerId)
                return BadRequest();
            return Ok(new PetProfileEditContract
            {
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
            });
        }

        [HttpPut]
        [Route("edit")]
        public async Task<IActionResult> EditPetProfile(PetProfileEditContract model)
        {
            var pet = await _petService.GetById(model.Id);
            if (pet == null || User.Claims.FirstOrDefault(x => x.Type == "UserID").Value != pet.OwnerId)
                return BadRequest();
            pet.Breed = model.Breed;
            pet.DateOfBirth = model.DateOfBirth;
            pet.Gender = model.Gender;
            pet.Img = model.Img;
            pet.IsReadyForSex = model.IsReadyForSex;
            pet.IsSale = model.IsSale;
            pet.IsShare = model.IsShare;
            pet.Location = model.Location;
            pet.Name = model.Name;
            await _petService.Update(pet);
            return Ok(pet.Id);
        }

        [Route("delete")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();
            var pet = await _petService.GetById((int)id);
            if (pet == null || User.Claims.FirstOrDefault(x => x.Type == "UserID").Value != pet.OwnerId)
                return BadRequest();
            await _petService.Delete((int)id);
            return RedirectToAction("Index", "Home");
        }
    }
}