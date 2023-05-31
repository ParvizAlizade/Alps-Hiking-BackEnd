using Alps_Hiking.Services;
using Alps_Hiking.DAL;
using Alps_Hiking.Entities;
using Alps_Hiking.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Alps_Hiking.Controllers
{



    public class TourController : Controller
    {
        readonly AlpsHikingDbContext _context;
        private readonly UserManager<User> _userManager;
        public TourController(AlpsHikingDbContext context,UserManager<User>userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index(int destinationId)
        {
            IQueryable<Tour> alltour = _context.Tours
                .Include(t => t.Category)
                    .Include(t => t.TourDates)
                       .Include(t => t.TourImages)
                         .Include(t => t.Itineraries)
                           .Include(t => t.PassengerCounts)
                             .Include(t => t.Destination);


            if(destinationId != 0)
            {
                alltour = alltour.Where(x => x.DestinationId == destinationId);
            }

            List<Tour> tours=alltour.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Slider = _context.Sliders.ToList();
            return View(tours);
        }

        public IActionResult Details(int id)
        {
            if (id == 0) return NotFound();
            IQueryable<Tour> tours = _context.Tours.AsNoTracking().AsQueryable();
            Tour? tour = tours.Include(d => d.TourImages)
                .Include(d=>d.Category)
                .Include(d=>d.Destination)
                .Include(d=>d.PassengerCounts)
                .Include(d=>d.TourDates)
                .Include(d=>d.TourImages)
                .Include(d=>d.Comments).ThenInclude(u=>u.User)
                .Include(d=>d.Itineraries)
                                     .AsSingleQuery().FirstOrDefault(d => d.Id == id);

            ViewBag.Comment = _context.Comments.ToList();


            if (tour is null) return NotFound();
            return View(tour);
        }


        public async Task<IActionResult> AddComment(int id, Comment newComment)
        {
            if (newComment.Text is null)
            {
                return RedirectToAction(nameof(Details), new { id });
            }

            Tour tour = _context.Tours.Include(t => t.Comments).FirstOrDefault(t => t.Id == id);
            if (tour == null)
            {
                return NotFound();
            }

            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return Unauthorized();
            }

            Comment comment = new Comment
            {
                Text = newComment.Text,
                User = user,
                TimeStamp = DateTime.Now,
                TourId = tour.Id,
                Tour = tour
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }


        [HttpPost]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            Comment comment = _context.Comments.FirstOrDefault(c => c.Id == commentId);
            if (comment is not null)
            {
                User user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (comment.User == user)
                {
                    _context.Comments.Remove(comment);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction(nameof(Details), new { id = comment.TourId });
        }

    }
}
