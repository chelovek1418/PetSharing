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
    public interface IUserService : IDisposable
    {
        Task<IEnumerable<ChatDto>> GetChats(ClaimsPrincipal claims);
        IEnumerable<UserDto> FindUsers(string param);
        IEnumerable<RoleDto> GetRoles();
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
        Task<IdentityResult> CreateRole(string name);
        Task<IdentityResult> DeleteRole(string id);
        Task<IdentityResult> EditRole(RoleDto role);
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

        public async Task<IEnumerable<ChatDto>> GetChats(ClaimsPrincipal claims)
        {
            var id = (await Db.UserManager.GetUserAsync(claims)).Id;
            var messages = (await Db.UserManager.Users.Include(x => x.Messages).FirstOrDefaultAsync(d => d.Id == id)).Messages.OrderByDescending(m=>m.Date);
            List<ChatDto> Chats = new List<ChatDto>();
            foreach (var mes in messages)
            {
                if (!Chats.Any(x => x.Id == mes.ReceiverId))
                {
                    Chats.Add(new ChatDto { Id = mes.ReceiverId, Name =mes.User.UserName, Pic=mes.User.PicUrl, Date=mes.Date, LastMessage=mes.Text });
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
            var user = await Db.UserManager.GetUserAsync(claims);
            return user.ToDto();
        }

        public async Task<string> Update(UserDto dto)
        {
            User user = await Db.UserManager.FindByIdAsync(dto.Id);
            if (user != null)
            {
                var pers = dto.ToEntity();
                var role = await Db.UserManager.GetRolesAsync(pers);
                if (!(await Db.UserManager.RemoveFromRolesAsync(user, role)).Succeeded)
                    throw new ValidationException("Не удалить роль", "Role");
                if (!(await Db.UserManager.AddToRoleAsync(pers, dto.Role)).Succeeded)
                    throw new ValidationException("Не добавить роль", "Role");
                if (!(await Db.UserManager.UpdateAsync(pers)).Succeeded)
                    throw new ValidationException("Не удалось обновить", "");
                Db.Save();
                return user.Id;
            }
            throw new ValidationException("Пользователь не найден", "Id");
        }

        public async Task<UserDto> FindById(string id)
        {
            return (await Db.UserManager.FindByIdAsync(id)).ToDto();
        }

        public async Task<IdentityResult> Delete(UserDto dto)
        {
            var user = await Db.UserManager.FindByEmailAsync(dto.Email);
            if (user != null)
            {
                var result = Db.UserManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
                if (result == PasswordVerificationResult.Success)
                {
                    var delDate = await Db.UserManager.Users
                        .Include(x => x.Comments)
                        .Include(s=>s.Subscriptions)
                        .Include(y => y.Messages)
                        .Include(z => z.PetProfiles)
                        .ThenInclude(pp => pp.Posts)
                        .ThenInclude(p => p.Comments)
                        .FirstOrDefaultAsync(u => u.Id == user.Id);
                    delDate.Comments.ForEach(x=> Db.Comments.DeleteAsync(x.Id));
                    delDate.Subscriptions.RemoveRange(0, delDate.Subscriptions.Count); /*ForEach(x => delDate.Subscriptions.Remove(x))*/
                    delDate.Messages.ForEach(x => Db.Messages.Delete(x.Id));
                    delDate.PetProfiles.ForEach(x => 
                    { x.Posts.ForEach(y => 
                    { y.Comments.ForEach(c => Db.Comments.DeleteAsync(c.Id));
                        Db.Posts.DeleteAsync(y.Id);
                    });
                        Db.PetProfiles.DeleteAsync(x.Id);
                    });
                    var res = await Db.UserManager.DeleteAsync(user);
                    Db.Save();
                    return res;
                }
                throw new ValidationException("Неверный пароль", "Password");
            }
            throw new ValidationException("Пользователь не найден", "Id");
        }

        public async Task<IdentityResult> DeleteByAdmin(string id)
        {
            var user = await Db.UserManager.FindByIdAsync(id);
            if (user != null)
            {
                var res = await Db.UserManager.DeleteAsync(user);
                Db.Save();
                return res;
            }
            throw new ValidationException("Пользователь не найден", "Id");
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
            user = new User { Email = userDto.Email, UserName = userDto.UserName };
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
            await Db.SignInManager.SignInAsync(user, false);
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
            {
                throw new ValidationException("Невозможно отослать код на почту, так как email не был подтвержден", "Email");
            }
            return await Db.UserManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<string> Authenticate(UserDto model, bool rememberMe)
        {
            var user = await Db.UserManager.FindByEmailAsync(model.Email);
            if (user == null || !await Db.UserManager.CheckPasswordAsync(user, model.Password))
                throw new ValidationException("Пользоветель не найден", "Email");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("UserID", user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(5),
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
            {
                throw new ValidationException("Юзер не существует", "Email");
            }
            var result = await Db.UserManager.ResetPasswordAsync(user, code, dto.Password);
            if (!result.Succeeded)
            {
                throw new ValidationException("Не удалось установить новый пароль", "Password");
            }
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
            if (user != null)
            {
                var res = await Db.UserManager.ChangePasswordAsync(user, dto.Password, newPsw);
                Db.Save();
                return res;
            }
            else
            {
                throw new ValidationException("Пользователь не найден", "Id");
            }
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

        public async Task<IdentityResult> CreateRole(string name)
        {
            var res = await Db.RoleManager.CreateAsync(new IdentityRole(name));
            Db.Save();
            return res;
        }

        public async Task<IdentityResult> DeleteRole(string id)
        {
            var role = await Db.RoleManager.FindByIdAsync(id);
            if (role != null)
            {
                var res = await Db.RoleManager.DeleteAsync(role);
                Db.Save();
                return res;
            }
            throw new ValidationException("Роль не найдена", "Id");
        }

        public async Task<IdentityResult> EditRole(RoleDto role)
        {
            var result = await Db.RoleManager.FindByIdAsync(role.ToEntity().Id);
            if (result != null)
            {
                result.Name = role.Name;
                var res = await Db.RoleManager.UpdateAsync(result);
                Db.Save();
                return res;
            }
            throw new ValidationException("Роль не найдена", "Id");
        }

        public async Task<IEnumerable<PetProfileDto>> GetPetProfiles(string id)
        {
            return (await Db.UserManager.Users.Include(x => x.PetProfiles).FirstOrDefaultAsync(d => d.Id == id)).PetProfiles.Select(g => g.ToDto());
        }

        public async Task Subscribe(string userId, int petId)
        {
            var user =await Db.UserManager.FindByIdAsync(userId);
            if (user == null)
                throw new ValidationException("Пользователь не найден", "Id");
            user.Subscriptions.Add(new Subscription { PetId=petId, UserId=user.Id });
            Db.Save(); 
        }

        public async Task UnSubscribe(string userId, int petId)
        {
            var user =await Db.UserManager.Users.Include(s => s.Subscriptions).FirstOrDefaultAsync(x => x.Id == userId);
            var pet = await Db.PetProfiles.GetAsync(petId);
            if (user != null && pet != null)
            {
                var subscription = user.Subscriptions.FirstOrDefault(sc => sc.PetId == pet.Id);
                user.Subscriptions.Remove(subscription);
                Db.Save();
            }
        }

        public void Dispose()
        {
            Db.Dispose();
        }
    }
}
