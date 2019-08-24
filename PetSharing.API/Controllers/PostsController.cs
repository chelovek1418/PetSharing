using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class PostsController : ControllerBase
    {
        private IService<PostDto> _postService;
        private IService<PetProfileDto> _petService;
        private IUserService _userService;
        private ICommentService _commentService;

        public PostsController(IService<PostDto> postService, ICommentService commentService, IService<PetProfileDto> petService, IUserService userService)
        {
            _userService = userService;
            _petService = petService;
            _commentService = commentService;
            _postService = postService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            return Ok((await _postService.GetAll((page - 1) * 20)).Select(x => x.ToContract()).ToList());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int? id)
        {
            if (id == null)
                return BadRequest();
            var post = await _postService.GetById((int)id);
            if (post == null)
                return BadRequest();
            var postView = post.ToContract();
            postView.Comments = post.Comments.Select(x => x.ToContract()).ToList();
            var pet = _petService.GetById(post.PetId);
            if (pet == null)
                return BadRequest();
            postView.Pet = new PetProfileShortInfoContract
            {
                Id = pet.Result.Id,
                Name = pet.Result.Name,
                PicUrl = pet.Result.Img
            };
            return Ok(postView);
        }

        public IActionResult Create(int? id)
        {
            if (id == null)
                return BadRequest();
            return Ok(new CreatePostContract { PetId = (int)id });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePostContract post)
        {
            return RedirectToAction("GetById", "PetProfiles", $"{await _postService.Create(new PostDto { Date = DateTime.Now, Img = post.Img, PetId = post.PetId, Text = post.Text })}");
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return BadRequest();
            var post = await _postService.GetById((int)id);
            if (post == null)
                return BadRequest();
            return Ok(new EditPostContract { Id = post.Id, Text = post.Text, Img = post.Img });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(EditPostContract model)
        {
            var post = await _postService.GetById(model.Id);
            if (post == null)
                return BadRequest();
            await _postService.Update(new PostDto { Id = post.Id, Date = post.Date, Img = model.Img, Text = model.Text, PetId = post.PetId });
            return RedirectToAction("GetById", "PetProfiles", $"{post.PetId}");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();
            var petId = (await _postService.GetById((int)id)).PetId;
            await _postService.Delete((int)id);
            return RedirectToAction("GetById", "PetProfiles", $"{petId}");
        }

        //Comments

        public IActionResult CreateComment(int? id)
        {
            if (id == null)
                return BadRequest();
            return Ok(new CreateCommentContract { PostId = (int)id });
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(CreateCommentContract comment)
        {
            await _commentService.CreateAsync(new CommentDto
            {
                Date = DateTime.Now,
                PostId = comment.PostId,
                Text = comment.Text,
                UserId = (await _userService.GetCurrentUserAsync(HttpContext.User)).Id
            });
            return Ok(comment);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteComment(int? commentId)
        {
            if (commentId == null)
                return BadRequest();
            var postId = (await _commentService.GetAsync((int)commentId)).PostId;
            await _commentService.DeleteAsync((int)commentId);
            return RedirectToAction("GetById", "Posts", $"{postId}");
        }
    }
}