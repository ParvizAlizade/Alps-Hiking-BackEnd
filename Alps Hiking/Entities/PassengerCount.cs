namespace Alps_Hiking.Entities
{
    public class PassengerCount:BaseEntity
    {
        public byte Num { get; set; }
        public int TourId { get; set; }
        public Tour Tour { get; set; }
    }
}