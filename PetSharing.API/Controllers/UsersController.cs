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
    //[Authorize(Roles = "admin")]
    public class UsersController : ControllerBase
    {
        IUserService _userManager;

        public UsersController(IUserService userManager)
        {
            _userManager = userManager;
        }

        [Route("[action]")]
        public async Task<IActionResult> Index(int page = 1)
        {
            /*var users = */
            //users.ForEach(y => {
            //    y.PetProfiles = _userManager.GetPetProfiles(y.Id).Result.Select(d => d.ToContract()).ToList();
            //    y.Role = _userManager.GetRole(y.ToDto()).Result;
            //});
            return Ok((await _userManager.GetAll((page - 1) * 20)).Select(x => x.ToContract()).ToList());
        }

        public IActionResult Roles() => Ok(_userManager.GetRoles().Select(x => x.ToContract()).ToList());

        public IActionResult CreateUser() => Ok(_userManager.GetRoles().Select(x => x.ToContract()).ToList());

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserContract model)
        {
            if (ModelState.IsValid)
            {
                await _userManager.CreateByAdmin(new UserDto { Email = model.Email, UserName = model.UserName, Password=model.Password, Role = model.Role });
            }
            return Ok(model);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            await _userManager.DeleteByAdmin(id);
            return RedirectToAction("Index");
        }

        public IActionResult CreateRole() => Ok();

        [HttpPost]
        public async Task<IActionResult> CreateRole(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var result = await _userManager.CreateRole(name);
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return Ok(name);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRole(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            var result = await _userManager.DeleteRole(id);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return RedirectToAction("Roles");
        }

        [HttpPut]
        public async Task<IActionResult> EditRole(string id, RoleContract role)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            if (!(await _userManager.EditRole(new RoleDto { Id = id, Name = role.Name })).Succeeded)
            {
                Ok(role);
            }
            return RedirectToAction("Roles");
        }

        public async Task<IActionResult> Edit(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return NotFound();
            var user = await _userManager.FindById(userId);
            if (user != null)
            {
                var role = await _userManager.GetRole(user);
                var allRoles = _userManager.GetRoles().Select(x => x.ToContract()).ToList();
                ChangeRoleContract model = new ChangeRoleContract
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    Phone = user.Phone,
                    PicUrl = user.PicUrl,
                    Role = role,
                    AllRoles = allRoles,
                    Id = user.Id
                };
                return Ok(model);
            }
            return NotFound();
        }

        [HttpPut]
        public async Task<IActionResult> Edit(ChangeRoleContract model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindById(model.Id);
                if (user != null)
                {
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
            return Ok(model);
        }
    }
}