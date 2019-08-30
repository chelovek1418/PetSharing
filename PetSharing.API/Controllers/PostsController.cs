using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class PostsController : ControllerBase
    {
        private IService<PostDto> _postService;
        private IService<PetProfileDto> _petService;
        private ICommentService _commentService;

        public PostsController(IService<PostDto> postService, ICommentService commentService, IService<PetProfileDto> petService)
        {
            _petService = petService;
            _commentService = commentService;
            _postService = postService;
        }

        [HttpGet]
        [Route("index")]
        public async Task<IActionResult> Index(int page = 1)
        {
            return Ok((await _postService.GetAll((page - 1) * 20)).Select(x => x.ToContract()).ToList());
        }

        [Route("get")]
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

        [Route("create")]
        public IActionResult Create(int? id)
        {
            if (id == null)
                return BadRequest();
            return Ok(new CreatePostContract { PetId = (int)id });
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(CreatePostContract post)
        {
            return RedirectToAction("GetById", "PetProfiles", $"{await _postService.Create(new PostDto { Date = DateTime.Now, Img = post.Img, PetId = post.PetId, Text = post.Text })}");
        }

        [Route("edit")]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return BadRequest();
            var post = await _postService.GetById((int)id);
            if (post == null || (await _petService.GetById(post.PetId)).OwnerId != User.Claims.FirstOrDefault(x => x.Type == "UserID").Value)
                return BadRequest();
            return Ok(new EditPostContract { Id = post.Id, Text = post.Text, Img = post.Img });
        }

        [HttpPut]
        [Route("edit")]
        public async Task<IActionResult> Update(EditPostContract model)
        {
            var post = await _postService.GetById(model.Id);
            if (post == null || (await _petService.GetById(post.PetId)).OwnerId!= User.Claims.FirstOrDefault(x => x.Type == "UserID").Value)
                return BadRequest();
            post.Img = model.Img;
            post.Text = model.Text;
            await _postService.Update(post);
            return RedirectToAction("GetById", "PetProfiles", $"{post.PetId}");
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();
            var post = await _postService.GetById((int)id);
            if (post == null || (await _petService.GetById(post.PetId)).OwnerId != User.Claims.FirstOrDefault(x => x.Type == "UserID").Value)
                return BadRequest();
            await _postService.Delete((int)id);
            return RedirectToAction("GetById", "PetProfiles", $"{post.PetId}");
        }

        //Comments
        [Route("createcmnt")]
        public IActionResult CreateComment(int? id)
        {
            if (id == null)
                return BadRequest();
            return Ok(new CreateCommentContract { PostId = (int)id });
        }

        [HttpPost]
        [Route("createcmnt")]
        public async Task<IActionResult> CreateComment(CreateCommentContract comment)
        {
            await _commentService.CreateAsync(new CommentDto
            {
                Date = DateTime.Now,
                PostId = comment.PostId,
                Text = comment.Text,
                UserId = User.Claims.FirstOrDefault(x => x.Type == "UserID").Value
            });
            return Ok(comment);
        }

        [HttpDelete]
        [Route("deletecmnt")]
        public async Task<IActionResult> DeleteComment(int? id)
        {
            if (id == null)
                return BadRequest();
            var cmnt = await _commentService.GetAsync((int)id);
            if(cmnt == null|| cmnt.UserId!= User.Claims.FirstOrDefault(x => x.Type == "UserID").Value || 
                (await _petService.GetById((await _postService.GetById(cmnt.PostId)).PetId)).OwnerId != User.Claims.FirstOrDefault(x => x.Type == "UserID").Value)
                return BadRequest();
            await _commentService.DeleteAsync((int)id);
            return RedirectToAction("GetById", "Posts", $"{cmnt.PostId}");
        }
    }
}