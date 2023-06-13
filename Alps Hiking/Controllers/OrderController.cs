using Alps_Hiking.DAL;
using Alps_Hiking.Entities;
using Alps_Hiking.Utilities;
using Alps_Hiking.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.ContentModel;

namespace Alps_Hiking.Controllers
{
    public class OrderController : Controller
    {
        private readonly AlpsHikingDbContext _context;
        private readonly UserManager<User> _userManager;

        public OrderController(AlpsHikingDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            OrderVM orderVM = new OrderVM();
            User user = null;
            ViewBag.Slider = _context.Sliders.ToList();

            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);

                if (user != null)
                {
                    orderVM.Email = user.Email;
                    orderVM.Fullname = user.Fullname;
                    orderVM.Username = user.UserName;

                    List<BasketItem> basket = _context.BasketItems
                        .Include(x => x.User)
                        .Include(x => x.TourDate)
                        .ThenInclude(x => x.Tour)
                        .Where(x => x.User.Id == user.Id)
                        .ToList();

                    orderVM.BasketItems = basket;

                    decimal totalPrice = 0;

                    foreach (var item in basket)
                    {
                        totalPrice += item.Count * item.Price;
                    }

                    orderVM.TotalPrice = totalPrice;
                }
            }

            return View(orderVM);
        }



        [HttpPost]
        public async Task<IActionResult> Index(OrderVM model)
        {
         
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            User user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Error", "Home");
            }
            List<BasketItem> basket = _context.BasketItems
               .Include(x => x.User)
               .Include(x => x.TourDate)
               .ThenInclude(x => x.Tour)
               .Where(x => x.User.Id == user.Id)
               .ToList();

            var order = new Order
            {
                FullName = model.Fullname,
                Email = model.Email,
                Address = model.Adress,
                Number = model.Number,
                CreatedAt = DateTime.Now,
                Status = OrderStatus.Pending,
                UserId = user.Id,
                TotalPrice = 0,
                OrderItems = new List<OrderItem>()
            };

            decimal totalPrice = 0;

            if (model.BasketItems != null)
            {
                foreach (BasketItem basketItem in basket)
                {
                    TourDate tourDate = await _context.TourDates
                        .Include(x => x.Tour)
                        .FirstOrDefaultAsync(psc => psc.Id == basketItem.TourDateId);

                    if (tourDate == null)
                    {
                        ModelState.AddModelError("", "Choose TourDate");
                        return View(model);
                    }

                    if (tourDate.MaxPassengerCount<basketItem.Count)
                    {
                        _context.BasketItems.Remove(basketItem);
                        _context.TourDates.Remove(tourDate);
                        _context.SaveChanges();
                        return RedirectToAction("Index", "home");
                    }

                    if (basketItem.Count > tourDate.Tour.MaxPassengerCount)
                    {
                        ModelState.AddModelError("", "There aren't many places on the tour");
                        return View(model);
                    }

                    var orderItem = new OrderItem
                    {
                        SaleQuantity = basketItem.Count,
                        UnitPrice = (decimal)tourDate.Tour.DiscountPrice,
                        TourDateId = basketItem.TourDateId,
                        TourDate = tourDate,
                    };

                    order.OrderItems.Add(orderItem);

                    decimal itemTotalPrice = orderItem.UnitPrice * orderItem.SaleQuantity;
                    totalPrice += itemTotalPrice;

                    tourDate.MaxPassengerCount = (byte)(tourDate.MaxPassengerCount - basketItem.Count);
                    BasketItem baskets = await _context.BasketItems.FirstOrDefaultAsync(x => x.Id == basketItem.Id);

                    if (baskets != null)
                    {
                        _context.BasketItems.Remove(baskets);
                        await _context.SaveChangesAsync();
                    }

                }
            }

            order.TotalPrice = totalPrice;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

    }
}
