namespace Alps_Hiking.Entities
{
    public class Itinerary:BaseEntity
    {
        public int Day_count { get; set; }
        public string Day { get; set; }
        public int TourId { get; set; }
        public Tour Tour { get; set; }

   
    }
}
