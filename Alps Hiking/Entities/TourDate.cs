namespace Alps_Hiking.Entities
{
    public class TourDate:BaseEntity
    {
        public string TourDates { get; set; }
        public int TourId { get; set; }
        public Tour Tour { get; set; }
        public int MaxPassengerCount { get;set; }
    }
}