﻿using System;
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

        //[HttpGet]
        //public IActionResult Index()
        //{
        //    return Ok();
        //}

        //[HttpGet("register")]
        //public IActionResult Register()
        //{
        //    return Ok();
        //}

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterContract model)
        {
            if (ModelState.IsValid)
            {
                var userDto = new UserDto
                {
                    Email = model.Email,
                    Password = model.Password,
                    UserName = model.UserName,
                    Role = "user"
                };
                var id = await _userService.Create(userDto);
                if (id != string.Empty)
                {
                    var token = await _userService.GenerateToken(id);
                    if (id != string.Empty)
                    {
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = id, code = token }, protocol: HttpContext.Request.Scheme);                    
                        EmailService emailService = new EmailService();
                        await emailService.SendEmailAsync(model.Email, "Confirm your account",
                            $"Подтвердите регистрацию на {"PetSharing"}, перейдя по ссылке: <a href='{callbackUrl}'>link</a>");
                        return Ok();
                    }
                    ModelState.AddModelError("Token", "Токен пуст");
                }
                else
                    ModelState.AddModelError("Id", "Пользователь не найден");
            }
            return Ok(model);
        }

        [HttpGet("confirmemail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                return BadRequest("Error");
            }
            var result = await _userService.ConfirnEmail(userId, code);
            if (result.Succeeded)
                return Ok();
            else
                return BadRequest("Error");
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginContract model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.Authenticate(new UserDto { Email = model.Email, Password = model.Password }, model.RememberMe);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return Ok(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogOff()
        {
            _userService.LogOff();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordContract model)
        {
            if (ModelState.IsValid)
            {
                var token = await _userService.GeneratePswToken(model.Email);
                var user = _userService.FindByEmail(model.Email);
                if (token == string.Empty || user.Result.Id == string.Empty)
                    return BadRequest("Пользоветель не найден");
                var callbackUrl = Url.Action("ResetPassword", "Account", new { id = user.Result.Id, code = token }, protocol: HttpContext.Request.Scheme);
                EmailService emailService = new EmailService();
                await emailService.SendEmailAsync(model.Email, "Reset Password",
                    $"Для сброса пароля пройдите по ссылке: <a href='{callbackUrl}'>link</a>");
                return Ok();
            }
            return Ok(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return Ok(code);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordContract model)
        {
            if (ModelState.IsValid)
            {
                await _userService.ResetPsw(new UserDto { Email = model.Email, Password = model.Password }, model.Code);
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Неправильный логин и (или) пароль");
            return Ok(model);
        }
    }
}