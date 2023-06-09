using Alps_Hiking.DAL;
using Alps_Hiking.Entities;
using Alps_Hiking.Utilities.Extensions;
using Alps_Hiking.Utilities.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace Alps_Hiking.Areas.AlpAdmin.Controllers
{
    [Area("AlpAdmin")]
    [Authorize(Roles = "Admin, Moderator")]
    public class DestinationController : Controller
    {
        private readonly AlpsHikingDbContext _context;
        private readonly IWebHostEnvironment _env;


        public DestinationController(AlpsHikingDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            IEnumerable<Destiantion> destiantions = _context.Destiantions.AsEnumerable();
            return View(destiantions);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(Destiantion newdestination)
        {
            bool isDuplicated = _context.Destiantions.Any(c => c.Name == newdestination.Name);
            if (isDuplicated)
            {
                ModelState.AddModelError("Name", "You cannot duplicate value");
                return View();
            }


            if (newdestination.Image == null)
            {
                ModelState.AddModelError("Image", "Please choose image");
                return View();
            }
            if (!newdestination.Image.IsValidFile("image/"))
            {
                ModelState.AddModelError("Image", "Please choose image type file");
                return View();
            }
            if (!newdestination.Image.IsValidLength(1))
            {
                ModelState.AddModelError("Image", "Image size must  be maximum 1MB");
                return View();
            }

            string imagesFolderPath = Path.Combine(_env.WebRootPath, "assets", "images");
            newdestination.ImagePath = await newdestination.Image.CreateImage(imagesFolderPath, "toursphotos");
            _context.Destiantions.Add(newdestination);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



        public IActionResult Edit(int id)
        {
            if (id == 0) return NotFound();

            Destiantion destiantion = _context.Destiantions.FirstOrDefault(s => s.Id == id);
            if (destiantion is null) return BadRequest();
            return View(destiantion);
        }



        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(int id, Destiantion edited)
        {
            if (id != edited.Id) return BadRequest();
            Destiantion? destination = _context.Destiantions.FirstOrDefault(c => c.Id == id);
            if (destination is null) return BadRequest();
            bool duplicate = _context.Destiantions.Any(c => c.Name == edited.Name && edited.Name != destination.Name);
            if (duplicate)
            {
                ModelState.AddModelError("Name", "You cannot duplicate destination name");
                return View(destination);
            }
            if (edited.Image is not null)
            {
                string imagesFolderPath = Path.Combine(_env.WebRootPath, "assets", "images");
                string filePath = Path.Combine(imagesFolderPath, "toursphotos", destination.ImagePath);
                FileUpload.DeleteImage(filePath);
                destination.ImagePath = await edited.Image.CreateImage(imagesFolderPath, "toursphotos");
            }

            destination.Name = edited.Name;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete(int id)
        {
            if (id == 0) return NotFound();
            Destiantion destiantion = _context.Destiantions.FirstOrDefault(c => c.Id == id);
            if (destiantion is null) return NotFound();
            return View(destiantion);
        }



        [HttpPost]
        public IActionResult Delete(int id, Destiantion delete)
        {
            if (id != delete.Id) return BadRequest();
            Destiantion destiantion = _context.Destiantions.FirstOrDefault(c => c.Id == id);
            if (destiantion is null) return NotFound();
            delete = _context.Destiantions.FirstOrDefault(_c => _c.Id == id);
            if (delete is null) return NotFound();
            _context.Destiantions.Remove(delete);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



        public IActionResult Details(int id)
        {
            if (id <= 0) return NotFound();
            Destiantion destiantions = _context.Destiantions.FirstOrDefault(c => c.Id == id);
            if (destiantions is null) return NotFound();
            return View(destiantions);
        }
    }

}
