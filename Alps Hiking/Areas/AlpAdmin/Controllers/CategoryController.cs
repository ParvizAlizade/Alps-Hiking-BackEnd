using Alps_Hiking.DAL;
using Alps_Hiking.Entities;
using Alps_Hiking.Utilities.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alps_Hiking.Areas.AlpAdmin.Controllers
{
    [Area("AlpAdmin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly AlpsHikingDbContext _context;

        public CategoryController(AlpsHikingDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> categories = _context.Categories.AsEnumerable();
            return View(categories);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(Category newCategory)
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
            bool isDuplicated = _context.Categories.Any(c => c.Name == newCategory.Name);
            if (isDuplicated)
            {
                ModelState.AddModelError("", "You cannot duplicate value");
                return View();
            }
            _context.Categories.Add(newCategory);
            _context.SaveChanges();


            return RedirectToAction(nameof(Index));
        }


        public IActionResult Edit(int id)
        {
            if (id == 0) return NotFound();
            Category category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category is null) return NotFound();
            return View(category);
        }



        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(int id, Category edited)
        {
            if (id != edited.Id) return BadRequest();
            Category category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category is null) return NotFound();
            bool duplicate = _context.Categories.Any(c => c.Name == edited.Name && edited.Name != category.Name);
            if (duplicate)
            {
                ModelState.AddModelError("", "You cannot duplicate category name");
                return View(category);
            }
            category.Name = edited.Name;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



        public IActionResult Delete(int id)
        {
            if (id == 0) return NotFound();
            Category category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category is null) return NotFound();
            return View(category);
        }



        [HttpPost]
        public IActionResult Delete(int id, Category delete)
        {
            if (id != delete.Id) return BadRequest();
            Category category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category is null) return NotFound();
            delete = _context.Categories.FirstOrDefault(_c => _c.Id == id);
            if (delete is null) return NotFound();
            _context.Categories.Remove(delete);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



        public IActionResult Details(int id)
        {
            if (id <= 0) return NotFound();
            Category categories = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (categories is null) return NotFound();
            return View(categories);
        }
    }
}
