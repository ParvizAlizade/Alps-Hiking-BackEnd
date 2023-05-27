using Alps_Hiking.DAL;
using Alps_Hiking.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Alps_Hiking.Controllers
{
    public class AboutController : Controller
    {
        readonly AlpsHikingDbContext _context;
        public AboutController(AlpsHikingDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            HomeViewModel VM = new HomeViewModel
            {
                Slider = _context.Sliders.ToList(),
            };
            return View(VM);
        }
    }
}
