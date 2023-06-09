using Alps_Hiking.DAL;
using Alps_Hiking.Entities;
using Alps_Hiking.Utilities.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Alps_Hiking.Areas.AlpAdmin.Controllers
{
    [Area("AlpAdmin")]
    [Authorize(Roles = "Admin, Moderator")]
    public class ItineraryController : Controller
    {
        private readonly AlpsHikingDbContext _context;

        public ItineraryController(AlpsHikingDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<Itinerary> itineraries = _context.Itineraries
                                                        .Include(c => c.Tour).AsEnumerable();
            return View(itineraries);
        }



        public IActionResult Create()
        {
            ViewBag.Tours = _context.Tours.ToList();
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(Itinerary newitinerary)
        {
            ViewBag.Tours = _context.Tours.ToList();
            Tour? tour = _context.Tours.FirstOrDefault(x => x.Id == newitinerary.TourId);
       
            bool isDuplicated = _context.Itineraries.Any(c => c.Day == newitinerary.Day);
            if (isDuplicated)
            {
                ModelState.AddModelError("", "You cannot duplicate value");
                return View();
            }
            if (newitinerary.Day_count==0)
            {
                ModelState.AddModelError("", "You must choose DayCount");
            }
            Itinerary itinerary = new()
            {
                Day = newitinerary.Day,
                Day_count = newitinerary.Day_count,
                TourId=tour.Id,
                Tour=tour,
            };
            _context.Itineraries.Add(itinerary);
            _context.SaveChanges();


            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Tours = _context.Tours.ToList();
            if (id == 0) return NotFound();
            Itinerary itinerary = _context.Itineraries.Include(i=>i.Tour).FirstOrDefault(c => c.Id == id);
            if (itinerary is null) return NotFound();
            return View(itinerary);
        }



        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(int id, Itinerary edited)
        {
            ViewBag.Tours = _context.Tours.ToList();
            if (id != edited.Id) return BadRequest();
            Itinerary itinerary = _context.Itineraries.Include(c => c.Tour).FirstOrDefault(c => c.Id == id);
            if (itinerary is null) return NotFound();
            itinerary.Day = edited.Day;
            itinerary.Tour.Name = edited.Tour.Name;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete(int id)
        {
            if (id == 0) return NotFound();
            Itinerary itinerary = _context.Itineraries.Include(i=>i.Tour).FirstOrDefault(t => t.Id == id);
            if (itinerary is null) return NotFound();
            return View(itinerary);
        }


        [HttpPost]
        public IActionResult Delete(int id, Itinerary delete)
        {
            if (id != delete.Id) return BadRequest();
            Itinerary itinerary = _context.Itineraries.FirstOrDefault(c => c.Id == id);
            if (itinerary is null) return NotFound();
            delete = _context.Itineraries.FirstOrDefault(t => t.Id == id);
            if (delete is null) return NotFound();
            _context.Itineraries.Remove(delete);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            if (id <= 0) return NotFound();
            Itinerary itinerary = _context.Itineraries.Include(i=>i.Tour).FirstOrDefault(c => c.Id == id);
            if (itinerary is null) return NotFound();
            return View(itinerary);
        }
    }
}
