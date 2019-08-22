using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PetSharing.API.Extensions;
using PetSharing.Contracts;
using PetSharing.Contracts.PetProfilesContract;
using PetSharing.Contracts.UserContracts;
using PetSharing.Domain.Dtos;
using PetSharing.Domain.Models;
using PetSharing.Domain.Services;
using System.Linq;
using System.Threading.Tasks;

namespace PetSharing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        readonly IService<PostDto> _postService;
        readonly IUserService _userManager;

        public HomeController(IUserService userManager, IService<PostDto> postService)
        {
            _postService = postService;
            _userManager = userManager;
        }
        
        public async Task<IActionResult> Index(int page = 1)
        {
            var user = await _userManager.GetCurrentUserAsync(User);
            if (user != null)
            {
                return Ok(new HomeContract
                {
                    //User = new UserFullInfoContract
                    //{
                        //Id = user.Id,
                        Email=user.Email,
                        Phone=user.Phone,
                        FullName = user.FullName,
                        PicUrl = user.PicUrl,
                        UserName = user.UserName
                    //},
                    //Pets = (await _userManager.GetPetProfiles(user.Id)).Select(x => new PetProfileShortInfoContract { Id = x.Id, Name = x.Name, PicUrl = x.Img }).ToList(),
                    //Posts = (await _postService.GetBySub((page-1)*20, user.Id)).Select(x=>x.ToContract()).ToList()
                });                
            }
            return NotFound();
        }
    }
}