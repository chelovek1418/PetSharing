using PetSharing.Contracts;
using PetSharing.Contracts.PetProfilesContract;
using PetSharing.Domain.Dtos;

namespace PetSharing.API.Extensions
{
    public static class PostExtensions
    {
        public static PostContract ToContract(this PostDto dto)
        {
            if (dto == null)
                return null;
            return new PostContract()
            {
                //Pet = new PetProfileShortInfoContract
                //{
                //    Id=dto.Id,
                //    f
                //},
                Id = dto.Id,
                Date = dto.Date,
                Img = dto.Img,
                Text = dto.Text,
                LikeCount = dto.LikeCount
            };
        }
        public static PostDto ToDto(this PostContract post)
        {
            if (post == null)
                return null;
            return new PostDto()
            {
                Id = post.Id,
                PetId = post.Pet.Id,
                Date = post.Date,
                Img = post.Img,
                Text = post.Text,
                LikeCount = post.LikeCount
            };
        }
    }
}
