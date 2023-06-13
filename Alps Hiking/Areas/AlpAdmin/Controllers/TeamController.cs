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
    public class TeamController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly AlpsHikingDbContext _context;

        public TeamController(AlpsHikingDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index(int page=1)
        {

            ViewBag.TotalPage = Math.Ceiling((double)_context.Teams.Count() / 12);
            ViewBag.CurrentPage = page;
            ViewBag.Teams = _context.Teams
                               .AsNoTracking().Skip((page - 1) * 12).Take(12).AsEnumerable();
            IEnumerable<Team> teams = _context.Teams.Include(c => c.Profession).AsNoTracking().Skip((page - 1) * 12).Take(12).AsEnumerable();
            return View(teams);
            }

        public IActionResult Create()
        {
            ViewBag.Profession = _context.Profession.ToList();
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> CreateAsync(Team newteam)
        {
            ViewBag.Profession = _context.Profession.ToList();
            Profession? profession = _context.Profession.FirstOrDefault(p => p.Id == newteam.ProfessionId);

            Team team1 = new()
            {
                Profession = profession,
                ProfessionId= newteam.ProfessionId,
                About = newteam.About,
                Name = newteam.Name,
            };
            if (newteam.Image is null)
            {
                return View();
            }
            string imagesFolderPath = Path.Combine(_env.WebRootPath, "assets", "images");
            team1.ImagePath = await newteam.Image.CreateImage(imagesFolderPath, "teammembers");
            _context.Teams.Add(team1);      
            _context.SaveChanges();


            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Profession = _context.Profession.ToList();

            if (id == 0) return NotFound();
            Team team = _context.Teams.Include(i => i.Profession).FirstOrDefault(c => c.Id == id);
            if (team is null) return NotFound();
            return View(team);
        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(int id, Team edited)
        {
            ViewBag.Profession = _context.Profession.ToList();

            if (id != edited.Id) return BadRequest();
            Team team = _context.Teams.Include(p => p.Profession).FirstOrDefault(p => p.Id == edited.Id);
            if (team is null) return NotFound();
            team.Name = edited.Name;
            team.Profession = edited.Profession;
            team.About=edited.About;
            team.ProfessionId = edited.ProfessionId;
           if (edited.Image is not null)
            {
                string imagesFolderPath = Path.Combine(_env.WebRootPath, "assets", "images");
                string filePath = Path.Combine(imagesFolderPath, "teammembers", team.ImagePath);
                team.ImagePath = await edited.Image.CreateImage(imagesFolderPath, "teammembers");
                FileUpload.DeleteImage(filePath);
                team.ImagePath = await edited.Image.CreateImage(imagesFolderPath, "teammembers");

            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            if (id == 0) return NotFound();
            Team team = _context.Teams.Include(t=>t.Profession).FirstOrDefault(t => t.Id == id);
            if (team is null) return NotFound();
            return View(team);
        }



        [HttpPost]
        public IActionResult Delete(int id, Team delete)
        {
            if (id != delete.Id) return BadRequest();
            Team team = _context.Teams.FirstOrDefault(c => c.Id == id);
            if (team is null) return NotFound();
            delete = _context.Teams.FirstOrDefault(_c => _c.Id == id);
            if (delete is null) return NotFound();
            _context.Teams.Remove(delete);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Details(int id)
        {
            if (id <= 0) return NotFound();
            Team team = _context.Teams.Include(t=>t.Profession).FirstOrDefault(c => c.Id == id);
            if (team is null) return NotFound();
            return View(team);
        }
    }
}
