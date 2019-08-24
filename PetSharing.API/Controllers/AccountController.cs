using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PetSharing.Contracts;
using PetSharing.Data.Entities;
using PetSharing.Domain.Infrastructure;
using PetSharing.Domain.Models;
using PetSharing.Domain.Services;

namespace PetSharing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterContract model)
        {
            //await _userService.CreateRole("user");
            //await _userService.CreateRole("admin");
            var userDto = new UserDto
            {
                Email = model.Email,
                Password = model.Password,
                UserName = model.UserName,
                Role = "user"
            };
            var id = await _userService.Create(userDto);
            if (string.IsNullOrEmpty(id))
                return BadRequest();
            var token = await _userService.GenerateToken(id);
            if (string.IsNullOrEmpty(token))
                return BadRequest();
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = id, code = token }, protocol: HttpContext.Request.Scheme);
            EmailService emailService = new EmailService();
            await emailService.SendEmailAsync(model.Email, "Confirm your account",
                $"Подтвердите регистрацию на \"PetSharing\", перейдя по ссылке: <a href='{callbackUrl}'>link</a>");
            return Ok();
        }

        [HttpGet("confirmemail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
                return BadRequest("Error");
            var result = await _userService.ConfirnEmail(userId, code);
            if (result.Succeeded)
                return Ok();
            else
                return BadRequest("Error");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginContract model)
        {
            var result = await _userService.Authenticate(new UserDto { Email = model.Email, Password = model.Password }, model.RememberMe);
            if (string.IsNullOrEmpty(result))
                return BadRequest();
            return Ok(new { token = result });
        }

        [HttpPost]
        public IActionResult LogOff()
        {
            _userService.LogOff();
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("forgotpassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordContract model)
        {
            var token = await _userService.GeneratePswToken(model.Email);
            var user = _userService.FindByEmail(model.Email);
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(user.Result.Id))
                return BadRequest("Пользоветель не найден");
            var callbackUrl = Url.Action("ResetPassword", "Account", new { id = user.Result.Id, code = token }, protocol: HttpContext.Request.Scheme);
            await new EmailService().SendEmailAsync(model.Email, "Reset Password",
                $"Для сброса пароля пройдите по ссылке: <a href='{callbackUrl}'>link</a>");
            return Ok(new { token });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("resetpassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordContract model)
        {
            await _userService.ResetPsw(new UserDto { Email = model.Email, Password = model.Password }, model.Code);
            return RedirectToAction("Index", "Home");
        }
    }
}