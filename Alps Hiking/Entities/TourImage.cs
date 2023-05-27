namespace Alps_Hiking.Entities
{
    public class TourImage:BaseEntity
    {
        public string Path { get; set; }
        public bool? IsMain { get; set; }
        public int TourId { get; set; }
        public Tour Tour { get; set; }
    }
}
