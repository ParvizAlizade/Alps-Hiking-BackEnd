using Alps_Hiking.DAL;
using Alps_Hiking.Entities;
using Alps_Hiking.Utilities.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Alps_Hiking.Areas.AlpAdmin.Controllers
{
    [Area("AlpAdmin")]
    [Authorize(Roles = "Admin")]
    public class TourDateController : Controller
    {
        private readonly AlpsHikingDbContext _context;

        public TourDateController(AlpsHikingDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page=1)
        {
            ViewBag.TotalPage = Math.Ceiling((double)_context.Partners.Count() / 18);
            ViewBag.CurrentPage = page;
            IEnumerable<TourDate> tourDates = _context.TourDates.Include(t=>t.Tour).AsNoTracking().Skip((page - 1) * 18).Take(18).AsEnumerable();
            return View(tourDates);
        }


        public IActionResult Create()
        {
            ViewBag.Tour = _context.Tours.Include(t=>t.TourDates).ToList();

            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(TourDate newtourdate)
        {
            ViewBag.Tour = _context.Tours.ToList();
            if (!ModelState.IsValid)
            {
                foreach (string message in ModelState.Values.SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage))
                {
                    ModelState.AddModelError("", message);
                }

                return View();
            }
            bool isDuplicated = _context.TourDates.Any(c => c.TourDates == newtourdate.TourDates);
            if (isDuplicated)
            {
                ModelState.AddModelError("", "You cannot duplicate value");
                return View();
            }
            _context.TourDates.Add(newtourdate);
            _context.SaveChanges();


            return RedirectToAction(nameof(Index));
        }


        public IActionResult Edit(int id)
        {
            if (id == 0) return NotFound();
            TourDate tourDate = _context.TourDates.FirstOrDefault(c => c.Id == id);
            if (tourDate is null) return NotFound();
            return View(tourDate);
        }



        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(int id, TourDate edited)
        {
            if (id != edited.Id) return BadRequest();
            TourDate tourDate = _context.TourDates.FirstOrDefault(c => c.Id == id);
            if (tourDate is null) return NotFound();
            bool duplicate = _context.TourDates.Any(c => c.TourDates == edited.TourDates && edited.TourDates != tourDate.TourDates);
            if (duplicate)
            {
                ModelState.AddModelError("", "You cannot duplicate Tour Date");
                return View(tourDate);
            }
            tourDate.TourDates = edited.TourDates;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



        public IActionResult Delete(int id)
        {
            if (id == 0) return NotFound();
            TourDate tourDate = _context.TourDates.Include(t=>t.Tour).FirstOrDefault(c => c.Id == id);
            if (tourDate is null) return NotFound();
            return View(tourDate);
        }



        [HttpPost]
        public IActionResult Delete(int id, TourDate delete)
        {
            if (id != delete.Id) return BadRequest();
            TourDate tourDate = _context.TourDates.FirstOrDefault(c => c.Id == id);
            if (tourDate is null) return NotFound();
            delete = _context.TourDates.FirstOrDefault(_c => _c.Id == id);
            if (delete is null) return NotFound();
            _context.TourDates.Remove(delete);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



        public IActionResult Details(int id)
        {
            if (id <= 0) return NotFound();
            TourDate tourDate = _context.TourDates.Include(t=>t.Tour).FirstOrDefault(c => c.Id == id);
            if (tourDate is null) return NotFound();
            return View(tourDate);
        }
    }
}
