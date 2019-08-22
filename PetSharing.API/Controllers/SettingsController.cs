using System;
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

        public IActionResult Index()
        {
            return Ok();
        }

        [Authorize]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetCurrentUserAsync(HttpContext.User);
            if (user != null)
            {
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
            return NotFound();
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Edit(EditUserContract model)
        {
            if (ModelState.IsValid)
            {
                var id = await _userManager.Update(new UserDto { Email = model.Email, FullName = model.FullName, Id = model.Id, Phone = model.Phone, PicUrl = model.PicUrl, UserName = model.UserName, Role="user"});
                return RedirectToAction("Index", "Home");
            }
            return Ok(model);
        }

        [Authorize]
        public IActionResult Delete() => Ok();

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete(DeleteContract model)
        {
            if (ModelState.IsValid)
            {
                await _userManager.Delete(new UserDto { Email = model.Email, Password = model.Password });
            }
            ModelState.AddModelError("Email/Password", "Email или/(и) пароль неверны");
            return Ok(model);
        }

        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetCurrentUserAsync(HttpContext.User);
            if (user == null)
            {
                return NotFound();
            }
            var model = new ChangePasswordContract { Id = user.Id, Email = user.Email };
            return Ok(model);
        }

        [HttpPut]
        public async Task<IActionResult> ChangePassword(ChangePasswordContract model)
        {
            if (ModelState.IsValid)
            {
                if ((await _userManager.ChangePsw(new UserDto { Id = model.Id, Email = model.Email, Password = model.OldPassword }, model.NewPassword)).Succeeded)
                    return RedirectToAction("Index", "Home");
                ModelState.AddModelError("", "Не удалось установить новый пароль");
            }
            return Ok(model);
        }
    }
}