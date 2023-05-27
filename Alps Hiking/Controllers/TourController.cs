using Alps_Hiking.DAL;
using Alps_Hiking.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace Alps_Hiking.Controllers
{



    public class TourController : Controller
    {
        readonly AlpsHikingDbContext _context;
        public TourController(AlpsHikingDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int destinationId)
        {
            IQueryable<Tour> alltour = _context.Tours
                .Include(t => t.Category)
                    .Include(t => t.TourDates)
                       .Include(t => t.TourImages)
                         .Include(t => t.Itineraries)
                           .Include(t => t.PassengerCounts)
                             .Include(t => t.Destination);


            if(destinationId != 0)
            {
                alltour = alltour.Where(x => x.DestinationId == destinationId);
            }

            List<Tour> tours=alltour.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Slider = _context.Sliders.ToList();
            return View(tours);
        }

        public IActionResult Details(int id)
        {
            if (id == 0) return NotFound();
            IQueryable<Tour> tours = _context.Tours.AsNoTracking().AsQueryable();
            Tour? tour = tours.Include(d => d.TourImages)
                .Include(d=>d.Category)
                .Include(d=>d.Destination)
                .Include(d=>d.PassengerCounts)
                .Include(d=>d.TourDates)
                .Include(d=>d.TourImages)
                .Include(d=>d.Itineraries)
                                     .AsSingleQuery().FirstOrDefault(d => d.Id == id);

            if (tour is null) return NotFound();
            return View(tour);
        }

      

    }
}
