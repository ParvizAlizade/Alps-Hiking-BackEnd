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
        [Authorize(Roles = "Admin,Moderator")]

        public class PartnerController : Controller
        {
            private readonly AlpsHikingDbContext _context;
            private readonly IWebHostEnvironment _env;

            public PartnerController(AlpsHikingDbContext context, IWebHostEnvironment env)
            {
                _context = context;
                _env = env;
            }

            public IActionResult Index()
            {
                IEnumerable<Partner> partners = _context.Partners.AsEnumerable();
                return View(partners);
            }

            public IActionResult Create()
            {
                return View();
            }

            [HttpPost]
            [AutoValidateAntiforgeryToken]
            public async Task<IActionResult> Create(Partner newpartner)
            {

                if (newpartner.Image == null)
                {
                    ModelState.AddModelError("Image", "Please choose image");
                    return View();
                }
                if (!newpartner.Image.IsValidFile("image/"))
                {
                    ModelState.AddModelError("Image", "Please choose image type file");
                    return View();
                }
                if (!newpartner.Image.IsValidLength(1))
                {
                    ModelState.AddModelError("Image", "Image size must  be maximum 1MB");
                    return View();
                }

                string imagesFolderPath = Path.Combine(_env.WebRootPath, "assets", "images");
                newpartner.ImagePath = await newpartner.Image.CreateImage(imagesFolderPath, "partners");
                _context.Partners.Add(newpartner);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }



            public IActionResult Edit(int id)
            {
                if (id == 0) return NotFound();

                Partner partner = _context.Partners.FirstOrDefault(s => s.Id == id);
                if (partner is null) return BadRequest();
                return View(partner);
            }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Partner edited)
        {
            if (id != edited.Id)
                return BadRequest();

            Partner partner = _context.Partners.FirstOrDefault(c => c.Id == id);
            if (partner is null)
                return BadRequest();

            if (edited.Image is not null)
            {
                string imagesFolderPath = Path.Combine(_env.WebRootPath, "assets", "images");
                string filePath = Path.Combine(imagesFolderPath, "partners", partner.ImagePath);
                FileUpload.DeleteImage(filePath);

                string newImagePath = await edited.Image.CreateImage(imagesFolderPath, "partners");
                if (newImagePath is not null)
                {
                    partner.ImagePath = newImagePath;
                }
                else
                {
                    return RedirectToAction("Error", "Home");
                }
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete(int id)
            {
                if (id == 0) return NotFound();
                Partner partner = _context.Partners.FirstOrDefault(c => c.Id == id);
                if (partner is null) return NotFound();
                return View(partner);
            }


            [HttpPost]
            public IActionResult Delete(int id, Partner delete)
            {
                if (id != delete.Id) return BadRequest();
                Partner partner = _context.Partners.FirstOrDefault(c => c.Id == id);
                if (partner is null) return NotFound();
                delete = _context.Partners.FirstOrDefault(_c => _c.Id == id);
                if (delete is null) return NotFound();
                _context.Partners.Remove(delete);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            public IActionResult Details(int id)
            {
                if (id <= 0) return NotFound();
                Partner partner = _context.Partners.FirstOrDefault(c => c.Id == id);
                if (partner is null) return NotFound();
                return View(partner);
            }
        }
    }