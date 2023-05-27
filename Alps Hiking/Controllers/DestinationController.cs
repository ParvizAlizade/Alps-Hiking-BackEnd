using Alps_Hiking.DAL;
using Alps_Hiking.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Alps_Hiking.Controllers
{
    public class DestinationController : Controller
    {
        readonly AlpsHikingDbContext _context;
        public DestinationController(AlpsHikingDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            HomeViewModel VM = new HomeViewModel
            {
                Slider = _context.Sliders.Where(d=>d.Title=="Destinations").ToList(),
                Destiantion = _context.Destiantions.ToList(),
            };
            return View(VM);
        }
    }
}
