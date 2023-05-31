namespace Alps_Hiking.Entities
{
    public class Comment:BaseEntity
    {
        public string Text { get; set; }
        public DateTime TimeStamp { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int TourId { get; set; }
        public Tour Tour { get; set; }
        public int Rating { get; set; }
    }
}
