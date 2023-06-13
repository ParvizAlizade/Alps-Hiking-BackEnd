using Alps_Hiking.Entities;
using Alps_Hiking.Utilities.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using Alps_Hiking.DAL;
using Microsoft.EntityFrameworkCore;

namespace Alps_Hiking.Areas.AlpAdmin.Controllers
{
    [Area("AlpAdmin")]
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly AlpsHikingDbContext _context;

        public OrdersController(AlpsHikingDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page=1)
        {
            ViewBag.TotalPage = Math.Ceiling((double)_context.Orders.Count() / 12);
            ViewBag.CurrentPage = page;
            IEnumerable<Order> orders = _context.Orders.OrderByDescending(o=>o.CreatedAt).AsNoTracking().Skip((page - 1) * 12).Take(12).AsEnumerable();
            return View(orders);
        }

        public IActionResult Details(int id)
        {
            Order? order = _context.Orders.Include(o => o.User).
                Include(o => o.OrderItems).
                ThenInclude(o => o.TourDate.Tour).
                FirstOrDefault(o => o.Id == id);

            if (order is null) return NotFound();
            return View(order);
        }

        public IActionResult AcceptOrdersInfo(int id)
        {
            Order order = _context.Orders.Include(x => x.OrderItems).FirstOrDefault(x => x.Id == id);

            User user = _context.Users.FirstOrDefault(x => x.Id == order.UserId);
            if (order is null) return NotFound();

            order.Status = Utilities.OrderStatus.Accepted;

            _context.SaveChanges();

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("alpshiking1994@gmail.com", "Alps Hiking");
            mail.To.Add(new MailAddress(order.Email));
            mail.Subject = "Order Information";
            mail.Body = "Thank you for your order! Your order is ready ! Your Orders Total price is $" + order.TotalPrice;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("alpshiking1994@gmail.com", "jipcexnfxtvvsdiy");
            smtp.Send(mail);

            return RedirectToAction("index");
        }

        public IActionResult RejectsOrdersInfo(int id)
        {
            Order order = _context.Orders.FirstOrDefault(x => x.Id == id);

            User user = _context.Users.FirstOrDefault(x => x.Id == order.UserId);
            if (order is null) return NotFound();

            order.Status = Utilities.OrderStatus.Rejected;

            _context.SaveChanges();


            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("alpshiking1994@gmail.com", "Alps Hiking");
            mail.To.Add(new MailAddress(order.Email));
            mail.Subject = "Order Information";
            mail.Body = "Unfortunately,We are very sorry that your order was rejected";
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("alpshiking1994@gmail.com", "jipcexnfxtvvsdiy");
            smtp.Send(mail);
            return RedirectToAction("index");
        }
    }
}
