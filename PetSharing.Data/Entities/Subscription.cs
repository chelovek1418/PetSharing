namespace PetSharing.Data.Entities
{
    public class Subscription
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public int PetId { get; set; }
        public PetProfile PetProfile { get; set; }
    }
}
