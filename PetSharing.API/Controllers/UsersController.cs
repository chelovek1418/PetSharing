using Microsoft.AspNetCore.Mvc;
using PetSharing.Domain.Services;
using System.Linq;
using PetSharing.API.Extensions;
using PetSharing.Contracts;
using PetSharing.Domain.Models;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PetSharing.Data.Entities;
using System.Threading.Tasks;
using PetSharing.Contracts.AdminConracts;
using PetSharing.Domain.Dtos;

namespace PetSharingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UsersController : ControllerBase
    {
        IUserService _userManager;

        public UsersController(IUserService userManager)
        {
            _userManager = userManager;
        }

        [Route("index")]
        public async Task<IActionResult> Index(int page = 1)
        {
            /*var users = */
            //users.ForEach(y => {
            //    y.PetProfiles = _userManager.GetPetProfiles(y.Id).Result.Select(d => d.ToContract()).ToList();
            //    y.Role = _userManager.GetRole(y.ToDto()).Result;
            //});
            return Ok((await _userManager.GetAll((page - 1) * 20)).Select(x => x.ToContract()).ToList());
        }

        [Route("roles")]
        public IActionResult Roles() => Ok(_userManager.GetRoles().Select(x => x.ToContract()).ToList());

        [Route("create")]
        public IActionResult CreateUser() => Ok(_userManager.GetRoles().Select(x => x.ToContract()).ToList());

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserContract model)
        {
            await _userManager.CreateByAdmin(new UserDto { Email = model.Email, UserName = model.UserName, Password = model.Password, Role = model.Role });
            return Ok(model);
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();
            await _userManager.DeleteByAdmin(id);
            return RedirectToAction("Index");
        }

        [Route("getinrole")]
        public async Task<IActionResult> GetUsersInRole(string name)
        {
            return Ok((await _userManager.GetUsersInRole(name)).Select(x => x.ToContract()).ToList());
        }

        [Route("edit")]
        public async Task<IActionResult> Edit(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return BadRequest("Id canot be empty");
            var user = await _userManager.FindById(Id);
            if (user == null)
                return BadRequest("User not found");
            var role = await _userManager.GetRole(user);
            var allRoles = _userManager.GetRoles().Select(x => x.ToContract()).ToList();
            return Ok(new ChangeRoleContract
            {
                Email = user.Email,
                UserName = user.UserName,
                FullName = user.FullName,
                Phone = user.Phone,
                PicUrl = user.PicUrl,
                Role = role,
                AllRoles = allRoles,
                Id = user.Id
            });
        }

        [HttpPut]
        [Route("edit")]
        public async Task<IActionResult> Edit(ChangeRoleContract model)
        {
            var user = await _userManager.FindById(model.Id);
            if (user == null)
                return BadRequest();
            user.Role = model.Role;
            user.Email = model.Email;
            user.UserName = model.UserName;
            user.FullName = model.FullName;
            user.Phone = model.Phone;
            user.PicUrl = model.PicUrl;
            await _userManager.Update(user);
            return RedirectToAction("Index");
        }
    }
}