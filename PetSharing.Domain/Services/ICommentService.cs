using PetSharing.Data.UnitOfWorks;
using PetSharing.Domain.Dtos;
using PetSharing.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetSharing.Domain.Services
{
    public interface ICommentService
    {
        Task<CommentDto> GetAsync(int id);
        Task DeleteAsync(int id);
        Task CreateAsync(CommentDto comment);
    }

    public class CommentService : ICommentService
    {
        IUnitOfWork _unitOfWork { get; set; }

        public CommentService(IUnitOfWork uow)
        {
            _unitOfWork = uow;
        }

        public async Task<CommentDto> GetAsync(int id) => (await _unitOfWork.Comments.GetAsync(id)).ToDto();

        public async Task DeleteAsync(int id) => await _unitOfWork.Comments.DeleteAsync(id);

        public async Task CreateAsync(CommentDto comment) => await _unitOfWork.Comments.CreateAsync(comment.ToEntity());
    }
}
