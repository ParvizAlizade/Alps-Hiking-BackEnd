
using Alps_Hiking.DAL;
using Alps_Hiking.Entities;
using Alps_Hiking.Utilities;
using Alps_Hiking.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Alps_Hiking.Controllers
{
    public class OrderController : Controller
    {
        private readonly AlpsHikingDbContext _context;
        private readonly UserManager<User> _userManager;

        public OrderController(AlpsHikingDbContext context,UserManager<User> userManager)
        {
            _context=context;
            _userManager=userManager;
        }
        public async Task<IActionResult> Index()
        {

            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            OrderVM model = new()
            {
                Fullname = user.Fullname,
                Username = user.UserName,
                Email = user.Email,
                BasketItems = _context.BasketItems.Include(p => p.TourDate).ThenInclude(t=>t.Tour).Where(c => c.UserId == user.Id).ToList()


            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(OrderVM orderVM)
        {
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            OrderVM model = new()
            {
                Fullname = orderVM.Fullname,
                Username = orderVM.Username,
                Adress=orderVM.Adress,
                Email = orderVM.Email,
                Number = orderVM.Number,
                BasketItems = _context.BasketItems.Include(p => p.TourDate).Where(c => c.UserId == user.Id).ToList()
            };
            if (!ModelState.IsValid) return View(model);
            if (model.BasketItems.Count == 0) return RedirectToAction("Index", "Home");


            Order order = new Order()
            {
                Address = orderVM.Adress,
                TotalPrice = 0,
                UserId = user.Id,
                Number = orderVM.Number,
                Status = OrderStatus.Pending,
            };


            foreach (BasketItem item in model.BasketItems)
            {

                OrderItem orderItem = new OrderItem
                {
                    UnitPrice = (decimal)item.TourDate.Tour.DiscountPrice,
                    TourDateId = item.TourDateId,
                    SaleQuantity = item.Count,
                    Order = order
                };
                order.TotalPrice += (decimal)item.TourDate.Tour.DiscountPrice * item.Count;
                _context.OrderItems.Add(orderItem);
            }
            _context.BasketItems.RemoveRange(model.BasketItems);
            _context.Orders.Add(order);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}
