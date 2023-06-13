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
    public class ProfessionController : Controller
    {
        private readonly AlpsHikingDbContext _context;

        public ProfessionController(AlpsHikingDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page=1)
        {
            ViewBag.TotalPage = Math.Ceiling((double)_context.Profession.Count() / 12);
            ViewBag.CurrentPage = page;
            ViewBag.Profession = _context.Profession
                               .AsNoTracking().Skip((page - 1) * 12).Take(12).AsEnumerable();
            IEnumerable<Profession> professions = _context.Profession.AsNoTracking().Skip((page - 1) * 12).Take(12).AsEnumerable();
            return View(professions);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(Profession newprofession)
        {
            if (!ModelState.IsValid)
            {
                foreach (string message in ModelState.Values.SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage))
                {
                    ModelState.AddModelError("", message);
                }

                return View();
            }
            bool isDuplicated = _context.Profession.Any(c => c.Name == newprofession.Name);
            if (isDuplicated)
            {
                ModelState.AddModelError("", "You cannot duplicate value");
                return View();
            }
            _context.Profession.Add(newprofession);
            _context.SaveChanges();


            return RedirectToAction(nameof(Index));
        }


        public IActionResult Edit(int id)
        {
            if (id == 0) return NotFound();
            Profession profession = _context.Profession.FirstOrDefault(c => c.Id == id);
            if (profession is null) return NotFound();
            return View(profession);
        }



        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(int id, Profession edited)
        {
            if (id != edited.Id) return BadRequest();
            Profession profession = _context.Profession.FirstOrDefault(c => c.Id == id);
            if (profession is null) return NotFound();
            bool duplicate = _context.Profession.Any(c => c.Name == edited.Name && edited.Name != profession.Name);
            if (duplicate)
            {
                ModelState.AddModelError("", "You cannot duplicate profession name");
                return View(profession);
            }
            profession.Name = edited.Name;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



        public IActionResult Delete(int id)
        {
            if (id == 0) return NotFound();
            Profession profession = _context.Profession.FirstOrDefault(c => c.Id == id);
            if (profession is null) return NotFound();
            return View(profession);
        }



        [HttpPost]
        public IActionResult Delete(int id, Profession delete)
        {
            if (id != delete.Id) return BadRequest();
            Profession profession = _context.Profession.FirstOrDefault(c => c.Id == id);
            if (profession is null) return NotFound();
            delete = _context.Profession.FirstOrDefault(_c => _c.Id == id);
            if (delete is null) return NotFound();
            _context.Profession.Remove(delete);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



        public IActionResult Details(int id)
        {
            if (id <= 0) return NotFound();
            Profession professions = _context.Profession.FirstOrDefault(c => c.Id == id);
            if (professions is null) return NotFound();
            return View(professions);
        }
    }
}
