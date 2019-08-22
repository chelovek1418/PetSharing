using System;
using System.Collections.Generic;
using System.Text;

namespace PetSharing.Contracts
{
    public class SearchContract
    {
        public bool? IsPet { get; set; }
        public string Type { get; set; }
        public string Breed { get; set; }
        public string Location { get; set; }
        public string Gender { get; set; }
        public bool? IsSale { get; set; }
        public bool? IsReadyForSex { get; set; }
        public bool? IsShare { get; set; }
    }
}
