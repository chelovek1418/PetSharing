using PetSharing.Contracts;
using PetSharing.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetSharing.API.Extensions
{
    public static class CommentExtensions
    {
        public static CommentContract ToContract(this CommentDto dto)
        {
            if (dto == null)
                return null;
            return new CommentContract()
            {
                 Date = dto.Date,
                 Id = dto.Id,
                 LikeCount= dto.LikeCount,
                 PostId = dto.PostId,
                 Text = dto.Text,
                 User = new Contracts.UserContracts.UserShortInfoContract{ Id = dto.UserId, UserName = dto.UserName, PicUrl = dto.UserPic }
            };
        }
        public static CommentDto ToDto(this CommentContract contract)
        {
            if (contract == null)
                return null;
            return new CommentDto()
            {
                Date = contract.Date,
                Id = contract.Id,
                LikeCount = contract.LikeCount,
                PostId = contract.PostId,
                Text = contract.Text,
                UserId  = contract.User.Id,
                UserName = contract.User.UserName,
                UserPic = contract.User.PicUrl                
            };
        }
    }
}
