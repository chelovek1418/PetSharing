using PetSharing.Domain.Dtos;
using System;
using System.Collections.Generic;

namespace PetSharing.Domain.Models
{
    public class PetProfileDto
    {
        public string OwnerId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
        public string Type { get; set; }
        public string Breed { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Location { get; set; }
        public double AvgLikeCount { get; set; }
        public bool IsSale { get; set; }
        public bool IsReadyForSex { get; set; }
        public bool IsShare { get; set; }
        public List<PostDto> Posts { get; set; }

        public PetProfileDto()
        {
            Posts = new List<PostDto>();
        }

    }
}
