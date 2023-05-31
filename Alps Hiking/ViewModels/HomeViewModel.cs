using Alps_Hiking.Entities;

namespace Alps_Hiking.ViewModels
{
    public class HomeViewModel
    {
        public List<Slider> Slider { get; set; }
        public  List<Destiantion>Destiantion { get; set; }
        public List<Tour>Tours  { get; set; }
        public List<Partner>Partners { get; set; }
        public List<Comment> Comments { get; set; }
    }
}