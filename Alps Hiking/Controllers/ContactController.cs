using Alps_Hiking.DAL;
using Alps_Hiking.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Alps_Hiking.Controllers
{
    public class ContactController : Controller
    {
        private readonly AlpsHikingDbContext _context;
        public ContactController(AlpsHikingDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Setting> settings = _context.Settings.ToList();
            ViewBag.Slider = _context.Sliders.ToList();


            return View(settings);
        }
    }
}
