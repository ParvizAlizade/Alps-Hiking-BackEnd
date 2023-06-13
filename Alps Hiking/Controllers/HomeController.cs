using Alps_Hiking.DAL;
using Alps_Hiking.Entities;
using Alps_Hiking.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Alps_Hiking.Controllers
{
	public class HomeController : Controller
	{

        readonly AlpsHikingDbContext _context;
        public HomeController(AlpsHikingDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
		{
           HomeViewModel VM = new HomeViewModel
           {
               Slider= _context.Sliders.ToList(),   
               Destiantion=_context.Destiantions.ToList(),
               Partners = _context.Partners.Take(6).ToList(),
               Comments = _context.Comments.Include(c=>c.User).Take(8).ToList(),
               Teams = _context.Teams.ToList(),

               Tours = _context.Tours
                 .Include(t=>t.Category)
                    .Include(t=>t.TourDates)
                       .Include(t=>t.TourImages)
                         .Include(t=>t.Itineraries)
                         .Include(t=>t.Comments)
                           .Include(t=>t.PassengerCounts)
                             .Include(t=>t.Destination)
                                .OrderBy(t => t.Price)
                                  .Take(4).ToList(),
           };  
            return View(VM);
		}
	
	}
}