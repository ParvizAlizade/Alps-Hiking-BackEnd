namespace Alps_Hiking.Entities
{
    public class Tour:BaseEntity
    {
        public string Name { get; set; }
        public byte DayCount{ get; set; }
        public byte Difficulty { get; set; }
        public string Pickup { get; set; }
        public byte PassangerAge { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public string Description { get; set; }
        public string Airport { get; set; }
        public string MeetingPoint { get; set; }
        public string Expect { get; set; }
        public List<TourImage> TourImages { get; set; }
        public string Map { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public double DiscountPrice { get; set; }
        public List<Itinerary> Itineraries { get; set; }
        public List<PassengerCount> PassengerCounts { get; set; }
        public int MaxPassengerCount { get; set; }
        public List<TourDate> TourDates { get; set; }
        public int DestinationId { get; set; }
        public Destiantion Destination { get; set; }
        public List<Comment> Comments { get; set; }
        public int Rate { get; set; }
        public Tour()
        {
            TourImages = new();
            Itineraries=new();
            PassengerCounts = new();
            TourDates=new();
            Comments=new();
        }
    }

}