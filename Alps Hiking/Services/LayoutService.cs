using Alps_Hiking.DAL;
using Alps_Hiking.Entities;
using Alps_Hiking.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Alps_Hiking.Services
{
    public class LayoutService
    {
        private readonly AlpsHikingDbContext _context;
        private readonly IHttpContextAccessor _accessor;
        public LayoutService(AlpsHikingDbContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
        }

        public List<Setting> GetSettings()
        {
            List<Setting> settings = _context.Settings.ToList();
            return settings;
        }

        public BasketVM GetBaskets()
        {
            BasketVM basketData = new BasketVM()
            {
                TotalPrice = 0,
                BasketItem = new(),
                Count = 0
            };
            if (_accessor.HttpContext.User.Identity.IsAuthenticated)
            {

                List<BasketItem> basketItems = _context.BasketItems.Include(x=>x.TourDate).Include(b => b.User).Where(b => b.User.UserName == _accessor.HttpContext.User.Identity.Name).ToList();
                foreach (BasketItem item in basketItems)
                {
                    Tour tour = _context.Tours.Include(t=>t.TourDates).FirstOrDefault(f => f.Id == item.TourDate.TourId);
                    if (tour != null)
                    {
                        BasketItem basket = new BasketItem()
                        {
                            Id=item.Id,
                            TourDate = (TourDate)tour.TourDates.FirstOrDefault(),
                            Count = item.Count
                        };
                        basket.TourDate.Tour.Price = tour.Price;
						basketData.BasketItem.Add(basket);
                        basketData.Count++;
                        basketData.TotalPrice += item.TourDate.Tour.DiscountPrice * item.Count;
					}
                }
            }

            return basketData;
        }

        public List<Tour> GetTours()
        {
            List<Tour>tours=_context.Tours.Include(t => t.Category)
                    .Include(t => t.TourDates)
                       .Include(t => t.TourImages)
                         .Include(t => t.Itineraries)
                           .Include(t => t.PassengerCounts)
                             .Include(t => t.Destination).ToList();
            return tours;
        }
    }
}
