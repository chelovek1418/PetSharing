using PetSharing.Data.Entities;
using PetSharing.Data.UnitOfWorks;
using PetSharing.Domain.Dtos;
using PetSharing.Domain.Extensions;
using PetSharing.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetSharing.Domain.Services
{
    public class PostService : IService<PostDto>
    {
        private IUnitOfWork _unitOfWork { get; set; }

        public PostService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PostDto>> GetBySub(int skip, string id)
        {
            return (await _unitOfWork.Posts.GetBySub(skip, id)).Select(x => x.ToDto());
        }

        public async Task<IEnumerable<PostDto>> GetAll(int skip)
        {
            return (await _unitOfWork.Posts.GetAllAsync(skip)).Select(x => x.ToDto());
        }

        public async Task<PostDto> GetById(int id)
        {
            var post = await _unitOfWork.Posts.GetAsync(id);
            if (post == null)
                throw new ValidationException("Профиль не найден", "Id");
            var postDto = post.ToDto();
            postDto.Comments = post.Comments.Select(x => x.ToDto()).OrderByDescending(y => y.Date).ToList();
            //postDto.Comments.ForEach(x =>
            //{
            //    x.UserName = post.Comments.FirstOrDefault(y => y.Id == x.Id).User.UserName;
            //    x.UserPic = post.Comments.FirstOrDefault(y => y.Id == x.Id).User.PicUrl;
            //});
            return postDto;
        }

        public async Task<int> Create(PostDto postDto)
        {
            return await _unitOfWork.Posts.CreateAsync(postDto.ToEntity());
        }

        public async Task Update(PostDto post)
        {
            var entity = await _unitOfWork.Posts.GetAsync(post.Id);
            entity.Text = post.Text;
            entity.Img = post.Img;
            await _unitOfWork.Posts.UpdateAsync(entity);
        }

        public async Task Delete(int id)
        {
            await _unitOfWork.Posts.DeleteAsync(id);
        }
    }
}