namespace Alps_Hiking.Entities
{
    public class WishListItem:BaseEntity
    {
        public int TourId { get; set; }
        public Tour Tour { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
