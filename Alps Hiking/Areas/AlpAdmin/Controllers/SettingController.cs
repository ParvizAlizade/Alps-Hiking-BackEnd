using Alps_Hiking.DAL;
using Alps_Hiking.Entities;
using Alps_Hiking.Utilities.Extensions;
using Alps_Hiking.Utilities.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Alps_Hiking.Areas.AlpAdmin.Controllers
{
    [Area("AlpAdmin")]
    [Authorize(Roles = "Admin")]
    public class SettingController : Controller
    {
        private readonly AlpsHikingDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SettingController(AlpsHikingDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1)
        {
            ViewBag.TotalPage = Math.Ceiling((double)_context.Settings.Count() / 15);
            ViewBag.CurrentPage = page;
            ViewBag.Setting = _context.Settings
                               .AsNoTracking().Skip((page - 1) * 15).Take(15).AsEnumerable();
            IEnumerable<Setting> settings = _context.Settings.AsNoTracking().Skip((page - 1) * 15).Take(15).AsEnumerable();
            return View(settings);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Setting newSetting)
        {
            if (!ModelState.IsValid)
            {
                foreach (string message in ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                {
                    ModelState.AddModelError("", message);
                }
                return View();
            }

            if (newSetting.Image != null && newSetting.Image.Length > 0)
            {
                var imagefolderPath = Path.Combine(_env.WebRootPath, "assets","images");
                newSetting.Value = await newSetting.Image.CreateImage(imagefolderPath, "setting");
            }

            bool IsDuplicate = _context.Settings.Any(c => c.Key == newSetting.Key);

            if (IsDuplicate)
            {
                ModelState.AddModelError("", "You cannot duplicate");
                return View();
            }

            _context.Settings.Add(newSetting);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }



        public IActionResult Edit(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            Setting setting = _context.Settings.FirstOrDefault(s => s.Id == id);
            if (setting is null)
            {
                return BadRequest();
            }
            return View(setting);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Setting edited)
        {
            if (id != edited.Id) return NotFound();
            Setting setting = _context.Settings.FirstOrDefault(s => s.Id == id);
            if (!ModelState.IsValid) return View(setting);
            _context.Entry<Setting>(setting).CurrentValues.SetValues(edited);

            if (edited.Image is not null)
            {
                var imagefolderPath = Path.Combine(_env.WebRootPath, "assets","images");
                string filepath = Path.Combine(imagefolderPath, "setting", setting.Value);
                FileUpload.DeleteImage(filepath);
                setting.Value = await edited.Image.CreateImage(imagefolderPath, "setting");
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Details(int id)
        {
            if (id == 0) return NotFound();
            Setting? setting = _context.Settings.FirstOrDefault(c => c.Id == id);
            return setting is null ? BadRequest() : View(setting);
        }



        public IActionResult Delete(int id)
        {
            if (id == 0) return NotFound();
            Setting? setting = _context.Settings.FirstOrDefault(c => c.Id == id);
            if (setting is null) return NotFound();
            return View(setting);
        }

        [HttpPost]
        public IActionResult Delete(int id, Setting deleted)
        {
            if (id != deleted.Id) return NotFound();
            Setting? setting = _context.Settings.FirstOrDefault(c => c.Id == id);
            if (setting is null) return NotFound();
            _context.Settings.Remove(setting);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
