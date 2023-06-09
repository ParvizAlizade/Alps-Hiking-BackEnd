namespace Alps_Hiking.Entities
{
    public class BasketItem:BaseEntity
    {
        public int TourDateId { get; set; }
        public TourDate TourDate { get; set; }
        public int Count { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
    }
}
