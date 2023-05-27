using Alps_Hiking.DAL;
using Alps_Hiking.Entities;
using Alps_Hiking.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Alps_Hiking.Controllers
{
    public class TeamController : Controller
    {
        readonly AlpsHikingDbContext _context;
        public TeamController(AlpsHikingDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            
            List<Team> teams = _context.Teams.Include(T=>T.Profession).ToList();
            ViewBag.Slider= _context.Sliders.ToList();

            return View(teams);
        }
    }
}
