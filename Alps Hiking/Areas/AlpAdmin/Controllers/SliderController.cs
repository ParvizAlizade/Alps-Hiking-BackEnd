using Alps_Hiking.DAL;
using Alps_Hiking.Entities;
using Alps_Hiking.Utilities.Extensions;
using Alps_Hiking.Utilities.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alps_Hiking.Areas.AlpAdmin.Controllers
{
    [Area("AlpAdmin")]
    [Authorize(Roles = "Admin")]
    public class SliderController : Controller
    {
        private readonly AlpsHikingDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(AlpsHikingDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            IEnumerable<Slider> sliders = _context.Sliders.AsEnumerable();
            return View(sliders);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Slider newSlider)
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

            bool isDuplicatedOrder = _context.Sliders.Any(s => s.Title == newSlider.Title);
            if (isDuplicatedOrder)
            {
                ModelState.AddModelError("Title", "You cannot duplicate Title");
                return View();
            }

            if (newSlider.Image is null)
            {
                ModelState.AddModelError("Image", "Please choose image");
                return View();
            }
            if (!newSlider.Image.IsValidLength(3))
            {
                ModelState.AddModelError("Image", "Image must be maximum 3MB");
                return View();
            }
            if (newSlider.Image == null)
            {
                ModelState.AddModelError("Image", "Please choose image");
                return View();
            }
            if (!newSlider.Image.IsValidFile("image/"))
            {
                ModelState.AddModelError("Image", "Please choose image type file");
                return View();
            }

            string imagesFolderPath = Path.Combine(_env.WebRootPath, "assets", "images");
            newSlider.ImagePath = await newSlider.Image.CreateImage(imagesFolderPath, "slider");
            _context.Sliders.Add(newSlider);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }




        public IActionResult Edit(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            Slider slider = _context.Sliders.FirstOrDefault(s => s.Id == id);
            if (slider is null)
            {
                return BadRequest();
            }
            return View(slider);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Slider edited)
        {
            if (id != edited.Id) return NotFound();
            Slider slider = _context.Sliders.FirstOrDefault(s => s.Id == id);
            if (!ModelState.IsValid) return View(slider);

            if (edited.Image is not null)
            {
                var imagefolderPath = Path.Combine(_env.WebRootPath, "assets", "images", "slider");
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(edited.Image.FileName);
                string filepath = Path.Combine(imagefolderPath, fileName);
                FileUpload.DeleteImage(filepath);
                slider.ImagePath = fileName;

                using (var fileStream = new FileStream(filepath, FileMode.Create))
                {
                    await edited.Image.CopyToAsync(fileStream);
                }
            }


            slider.Title = edited.Title;
            slider.Button = edited.Button;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Details(int id)
        {
            if (id == 0) return NotFound();
            Slider? slider = _context.Sliders.FirstOrDefault(s => s.Id == id);
            return slider is null ? BadRequest() : View(slider);
        }

        public IActionResult Delete(int id)
        {
            if (id == 0) return NotFound();
            Slider? slider = _context.Sliders.FirstOrDefault(s => s.Id == id);
            if (slider is null) return NotFound();
            return View(slider);
        }

        [HttpPost]
        public IActionResult Delete(int id, Slider deleteslider)
        {
            if (id != deleteslider.Id) return NotFound();
            Slider? slider = _context.Sliders.FirstOrDefault(s => s.Id == id);
            if (slider is null) return NotFound();
            var imagefolderPath = Path.Combine(_env.WebRootPath, "assets", "images");

            string filepath = Path.Combine(imagefolderPath, "slider", slider.ImagePath);
            FileUpload.DeleteImage(filepath);
            _context.Sliders.Remove(slider);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
