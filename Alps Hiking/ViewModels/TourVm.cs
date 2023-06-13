using Alps_Hiking.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alps_Hiking.ViewModels
{
    public class TourVm
    {
        public string Name { get; set; }
        public byte DayCount { get; set; }
        public byte Difficulty { get; set; }
        public string Pickup { get; set; }
        public byte PassangerAge { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public string Airport { get; set; }
        public string MeetingPoint { get; set; }
        public string Expect { get; set; }
        [NotMapped]
        public IFormFile? MainPhoto { get; set; }
        [NotMapped]
        public ICollection<IFormFile>? HoverPhoto { get; set; }
        [NotMapped]
        public List<TourImage> TourImages { get; set; }
        public string Map { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public double DiscountPrice { get; set; }
        public List<int> Itineraries { get; set; }
        public List<int> PassengerCounts { get; set; }
        public List<int> TourDates { get; set; }
        public List<int>ImagesId { get; set; }
        public int DestinationId { get; set; }


        public TourVm()
        {
            TourImages = new();
            Itineraries = new();
            PassengerCounts = new();
            TourDates = new();
            ImagesId = new();
        }
    }
}
