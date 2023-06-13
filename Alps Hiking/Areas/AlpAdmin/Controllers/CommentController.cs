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
    public class CommentController : Controller
    {
        private readonly AlpsHikingDbContext _context;

        public CommentController(AlpsHikingDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page=1)
        {
            ViewBag.TotalPage = Math.Ceiling((double)_context.Comments.Count() / 16);
            ViewBag.CurrentPage = page;
            ViewBag.Dresses = _context.Comments
                                 .AsNoTracking().Skip((page - 1) * 16).Take(16).AsEnumerable();
            IEnumerable<Comment> comments = _context.Comments
                                                        .AsNoTracking().Skip((page - 1) * 16).Take(16).AsEnumerable();
            return View(comments);
        }

        public IActionResult Edit(int id)
        {
            if (id == 0) return NotFound();
            Comment comment = _context.Comments.FirstOrDefault(c => c.Id == id);
            if (comment is null) return NotFound();
            return View(comment);
        }



        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(int id, Comment edited)
        {
            if (id != edited.Id) return BadRequest();
            Comment comment = _context.Comments.Include(c=>c.Tour).Include(c=>c.User).FirstOrDefault(c => c.Id == id);
            if (comment is null) return NotFound();
            bool duplicate = _context.Comments.Any(c => c.Text == edited.Text && edited.Text != comment.Text);
            if (duplicate)
            {
                ModelState.AddModelError("", "You cannot duplicate comment");
                return View(comment);
            }
            comment.Text = edited.Text;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            if (id <= 0) return NotFound();
            Comment comments = _context.Comments.Include(c=>c.Tour).Include(c=>c.User).FirstOrDefault(c => c.Id == id);
            if (comments is null) return NotFound();
            return View(comments);
        }

        public IActionResult Delete(int id)
        {
            if (id == 0) return NotFound();
            Comment comment = _context.Comments.Include(c => c.User).Include(c => c.Tour).FirstOrDefault(c => c.Id == id);
            if (comment is null) return NotFound();
            return View(comment);
        }



        [HttpPost]
        public IActionResult Delete(int id, Comment delete)
        {
            if (id != delete.Id) return BadRequest();
            Comment comment = _context.Comments.FirstOrDefault(c => c.Id == id);
            if (comment is null) return NotFound();
            delete = _context.Comments.FirstOrDefault(_c => _c.Id == id);
            if (delete is null) return NotFound();
            _context.Comments.Remove(delete);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}
