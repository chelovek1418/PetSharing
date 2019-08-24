﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetSharing.Contracts;
using PetSharing.Contracts.UserContracts;
using PetSharing.Domain.Models;
using PetSharing.Domain.Services;

namespace PetSharing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SettingsController : ControllerBase
    {
        IUserService _userManager;

        public SettingsController(IUserService userService)
        {
            _userManager = userService;
        }

        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetCurrentUserAsync(User);
            if (user == null)
                return BadRequest();
            return Ok(new EditUserContract
            {
                Email = user.Email,
                UserName = user.UserName,
                FullName = user.FullName,
                Phone = user.Phone,
                PicUrl = user.PicUrl,
                Id = user.Id
            });
        }

        [HttpPut]
        public async Task<IActionResult> Edit(EditUserContract model)
        {
            await _userManager.Update(new UserDto { Email = model.Email, FullName = model.FullName, Id = model.Id, Phone = model.Phone, PicUrl = model.PicUrl, UserName = model.UserName, Role = "user" });
            return RedirectToAction("Index", "Home");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(DeleteContract model)
        {
            if (!(await _userManager.Delete(new UserDto { Email = model.Email, Password = model.Password })).Succeeded)
                return BadRequest("Password or Email is wrong");
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetCurrentUserAsync(User);
            if (user == null)
                return BadRequest();
            var model = new ChangePasswordContract { Id = user.Id, Email = user.Email };
            return Ok(model);
        }

        [HttpPut]
        public async Task<IActionResult> ChangePassword(ChangePasswordContract model)
        {
            if ((await _userManager.ChangePsw(new UserDto { Id = model.Id, Email = model.Email, Password = model.OldPassword }, model.NewPassword)).Succeeded)
                return RedirectToAction("Index", "Home");
            return BadRequest();
        }
    }
}