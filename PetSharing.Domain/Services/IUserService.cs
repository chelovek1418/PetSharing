using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetSharing.Data.Entities;
using PetSharing.Data.UnitOfWorks;
using PetSharing.Domain.Dtos;
using PetSharing.Domain.Extensions;
using PetSharing.Domain.Infrastructure;
using PetSharing.Domain.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PetSharing.Domain.Services
{
    public interface IUserService
    {
        Task<IEnumerable<ChatDto>> GetChats(ClaimsPrincipal claims);
        IEnumerable<UserDto> FindUsers(string param);
        IEnumerable<RoleDto> GetRoles();
        Task<IEnumerable<UserDto>> GetUsersInRole(string name);
        Task<IdentityResult> ChangePsw(UserDto dto, string newPsw);
        Task<string> Update(UserDto dto);
        Task<IEnumerable<UserDto>> GetAll(int skip);
        Task<string> Create(UserDto userDto);
        Task<IdentityResult> CreateByAdmin(UserDto userDto);
        Task<string> GenerateToken(string id);
        Task<string> GeneratePswToken(string param);
        Task<string> Authenticate(UserDto userDto, bool rememberMe);
        Task<IdentityResult> ConfirnEmail(string id, string code);
        Task<IdentityResult> ResetPsw(UserDto user, string code);
        void LogOff();
        Task<UserDto> FindById(string id);
        Task<UserDto> FindByEmail(string email);
        Task<IdentityResult> Delete(UserDto dto);
        Task<IdentityResult> DeleteByAdmin(string id);
        Task<IEnumerable<PetProfileDto>> GetPetProfiles(string id);
        Task<string> GetRole(UserDto dto);
        Task<UserDto> GetCurrentUserAsync(ClaimsPrincipal claims);
        Task Subscribe(string userId, int petId);
        Task UnSubscribe(string userId, int petId);
    }
    public class UserService : IUserService
    {
        IUnitOfWork Db { get; set; }

        public UserService(IUnitOfWork uow)
        {
            Db = uow;
        }

        public async Task<IEnumerable<UserDto>> GetUsersInRole(string name)
        {
            return (await Db.UserManager.GetUsersInRoleAsync(name)).Select(x => x.ToDto());
        }

        public async Task<IEnumerable<ChatDto>> GetChats(ClaimsPrincipal claims)
        {
            var id = (await Db.UserManager.GetUserAsync(claims)).Id;
            var messages = (await Db.UserManager.Users.Include(x => x.Messages).FirstOrDefaultAsync(d => d.Id == id)).Messages.OrderByDescending(m => m.Date);
            List<ChatDto> Chats = new List<ChatDto>();
            foreach (var mes in messages)
            {
                if (!Chats.Any(x => x.Id == mes.ReceiverId))
                {
                    Chats.Add(new ChatDto { Id = mes.ReceiverId, Name = mes.User.UserName, Pic = mes.User.PicUrl, Date = mes.Date, LastMessage = mes.Text });
                }
            }
            return Chats;
        }

        public async Task<IEnumerable<UserDto>> GetAll(int skip)
        {
            return (await Db.UserManager.Users.Skip(skip).Take(20).ToListAsync()).Select(x => x.ToDto());
        }

        public IEnumerable<UserDto> FindUsers(string param)
        {
            List<User> Users = new List<User>();
            Users.AddRange(Db.UserManager.Users.Where(x => x.UserName.Contains(param)).ToList());
            Users.AddRange(Db.UserManager.Users.Where(x => x.FullName.Contains(param)).ToList());
            Users.AddRange(Db.UserManager.Users.Where(x => x.Email.Contains(param)).ToList());
            return Users.Distinct().Select(x => x.ToDto());
        }

        public async Task<UserDto> GetCurrentUserAsync(ClaimsPrincipal claims)
        {
            return (await Db.UserManager.FindByIdAsync(claims.Claims.FirstOrDefault(x => x.Type == "UserID").Value)).ToDto();
        }

        public async Task<string> Update(UserDto dto)
        {
            var user = await Db.UserManager.FindByIdAsync(dto.Id);
            if (user == null)
                throw new ValidationException("Пользователь не найден", "Id");
            user.Email = dto.Email;
            user.FullName = dto.FullName;
            user.PhoneNumber = dto.Phone;
            user.PicUrl = dto.PicUrl;
            user.UserName = dto.UserName;
            var role = await Db.UserManager.GetRolesAsync(user);
            if (!(await Db.UserManager.RemoveFromRolesAsync(user, role)).Succeeded)
                throw new ValidationException("Не удалось удалить роль", "Role");
            if (!(await Db.UserManager.AddToRoleAsync(user, dto.Role)).Succeeded)
                throw new ValidationException("Не удалось добавить роль", "Role");
            if (!(await Db.UserManager.UpdateAsync(user)).Succeeded)
                throw new ValidationException("Не удалось обновить", "");
            Db.Save();
            return user.Id;
        }

        public async Task<UserDto> FindById(string id)
        {
            return (await Db.UserManager.FindByIdAsync(id)).ToDto();
        }

        public async Task<IdentityResult> Delete(UserDto dto)
        {
            var user = await Db.UserManager.FindByIdAsync(dto.Id);
            if (user == null)
                throw new ValidationException("Пользователь не найден", "Id");
            if (Db.UserManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password) == PasswordVerificationResult.Failed)
                throw new ValidationException("Неверный пароль", "Password");
            var delDate = await Db.UserManager.Users
                .Include(x => x.Comments)
                .Include(s => s.Subscriptions)
                .Include(y => y.Messages)
                .Include(z => z.PetProfiles)
                .ThenInclude(pp => pp.Posts)
                .ThenInclude(p => p.Comments)
                .FirstOrDefaultAsync(u => u.Id == user.Id);
            delDate.Comments.ForEach(x => Db.Comments.DeleteAsync(x.Id));
            delDate.Subscriptions.RemoveRange(0, delDate.Subscriptions.Count); /*ForEach(x => delDate.Subscriptions.Remove(x))*/
            delDate.Messages.ForEach(x => Db.Messages.Delete(x.Id));
            delDate.PetProfiles.ForEach(x =>
            {
                x.Posts.ForEach(y =>
              {
                  y.Comments.ForEach(c => Db.Comments.DeleteAsync(c.Id));
                  Db.Posts.DeleteAsync(y.Id);
              });
                Db.PetProfiles.DeleteAsync(x.Id);
            });
            var res = await Db.UserManager.DeleteAsync(user);
            Db.Save();
            return res;
        }

        public async Task<IdentityResult> DeleteByAdmin(string id)
        {
            var user = await Db.UserManager.FindByIdAsync(id);
            if (user == null)
                throw new ValidationException("Пользователь не найден", "Id");
            var res = await Db.UserManager.DeleteAsync(user);
            Db.Save();
            return res;
        }

        public async Task<IdentityResult> ConfirnEmail(string id, string code)
        {
            var user = await Db.UserManager.FindByIdAsync(id);
            if (user == null)
                throw new ValidationException("Пользователь не найден", "Id");
            return await Db.UserManager.ConfirmEmailAsync(user, code);
        }

        public async Task<IdentityResult> CreateByAdmin(UserDto userDto)
        {
            User user = await Db.UserManager.FindByEmailAsync(userDto.Email);
            if (user != null)
                throw new ValidationException("Пользователь с таким электронным адрессом уже существует", "Email");
            user = new User { Email = userDto.Email, UserName = userDto.UserName, EmailConfirmed = true };
            var result = await Db.UserManager.CreateAsync(user, userDto.Password);
            if (!result.Succeeded)
                throw new ValidationException("Не удалось создать пользователя", "");
            if (!(await Db.UserManager.AddToRoleAsync(user, userDto.Role)).Succeeded)
                throw new ValidationException("Не удалось добавить роль для пользователя", "");
            Db.Save();
            return result;
        }

        public async Task<string> Create(UserDto userDto)
        {
            User user = await Db.UserManager.FindByEmailAsync(userDto.Email);
            if (user != null)
                throw new ValidationException("Пользователь с таким электронным адрессом уже существует", "Email");
            user = new User { Email = userDto.Email, UserName = userDto.UserName };
            if (!(await Db.UserManager.CreateAsync(user, userDto.Password)).Succeeded)
                throw new ValidationException("Не удалось создать пользователя", "");
            if (!(await Db.UserManager.AddToRoleAsync(user, userDto.Role)).Succeeded)
                throw new ValidationException("Не удалось добавить роль для пользователя", "");
            //await Db.SignInManager.SignInAsync(user, false);
            Db.Save();
            return user.Id;
        }

        public async Task<string> GenerateToken(string id)
        {
            var user = await Db.UserManager.FindByIdAsync(id);
            if (user != null)
                return await Db.UserManager.GenerateEmailConfirmationTokenAsync(user);
            throw new ValidationException("Пользователь не найден", "Id");
        }

        public async Task<string> GeneratePswToken(string param)
        {
            var user = await Db.UserManager.FindByEmailAsync(param);
            if (user == null || !user.EmailConfirmed)
                throw new ValidationException("Невозможно отослать код на почту, так как email не был подтвержден", "Email");
            return await Db.UserManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<string> Authenticate(UserDto model, bool rememberMe)
        {
            var user = await Db.UserManager.FindByEmailAsync(model.Email);
            if (user == null || !await Db.UserManager.CheckPasswordAsync(user, model.Password))
                throw new ValidationException("Пользоветель не найден", "Email");
            model.Id = user.Id;
            var role = await GetRole(model);
            var _opt = new IdentityOptions();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("UserID", user.Id.ToString()),
                    new Claim(_opt.ClaimsIdentity.RoleClaimType, role)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1234567891011121")), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);
            if (!await Db.UserManager.IsEmailConfirmedAsync(user))
                throw new ValidationException("Вы не подтвердили свой Email", " Email");
            return token;
            //return await Db.SignInManager.PasswordSignInAsync(user.UserName, model.Password, rememberMe, false);
        }

        public async Task<IdentityResult> ResetPsw(UserDto dto, string code)
        {
            var user = await Db.UserManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new ValidationException("Юзер не существует", "Email");
            var result = await Db.UserManager.ResetPasswordAsync(user, code, dto.Password);
            if (!result.Succeeded)
                throw new ValidationException("Не удалось установить новый пароль", "Password");
            Db.Save();
            return result;
        }

        public async Task<UserDto> FindByEmail(string email)
        {
            return (await Db.UserManager.FindByEmailAsync(email)).ToDto();
        }

        public async Task<IdentityResult> ChangePsw(UserDto dto, string newPsw)
        {
            var user = await Db.UserManager.FindByIdAsync(dto.Id);
            if (user == null)
                throw new ValidationException("Пользователь не найден", "Id");
            var res = await Db.UserManager.ChangePasswordAsync(user, dto.Password, newPsw);
            Db.Save();
            return res;
        }

        public async void LogOff()
        {
            await Db.SignInManager.SignOutAsync();
        }

        public async Task<string> GetRole(UserDto dto)
        {
            return (await Db.UserManager.GetRolesAsync(dto.ToEntity())).FirstOrDefault();
        }

        public IEnumerable<RoleDto> GetRoles()
        {
            return Db.RoleManager.Roles.Select(x => x.ToDto());
        }

        public async Task<IEnumerable<PetProfileDto>> GetPetProfiles(string id)
        {
            return (await Db.UserManager.Users.Include(x => x.PetProfiles).FirstOrDefaultAsync(d => d.Id == id)).PetProfiles.Select(g => g.ToDto());
        }

        public async Task Subscribe(string userId, int petId)
        {
            var pet = await Db.PetProfiles.GetAsync(petId);
            if (pet == null || pet.Folowers.FirstOrDefault(x=>x.UserId==userId)!=null)
                throw new ValidationException("Профиль не найден", "Id");            
            pet.Folowers.Add(new Subscription { PetId = petId, UserId = userId });
            Db.Save();
        }

        public async Task UnSubscribe(string userId, int petId)
        {
            var pet = await Db.PetProfiles.GetAsync(petId);
            if (pet == null || pet.Folowers.FirstOrDefault(x => x.UserId == userId) == null)
                throw new ValidationException("Профиль не найден", "Id");
            pet.Folowers.Remove(pet.Folowers.FirstOrDefault(f => f.UserId == userId));
            Db.Save();
        }
    }
}
