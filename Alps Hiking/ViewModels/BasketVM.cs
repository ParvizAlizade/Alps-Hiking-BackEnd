using Alps_Hiking.Entities;

namespace Alps_Hiking.ViewModels
{
    public class BasketVM
    {
        public List<BasketItem> BasketItem { get; set; }
        public double TotalPrice { get; set; }
        public int Count { get; set; }
    }
}
