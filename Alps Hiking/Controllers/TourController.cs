using Alps_Hiking.Services;
using Alps_Hiking.DAL;
using Alps_Hiking.Entities;
using Alps_Hiking.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Alps_Hiking.Controllers
{


    public class TourComparer : IEqualityComparer<Tour>
    {
        public bool Equals(Tour? x, Tour? y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;
            return x.Id == y.Id;
        }

        public int GetHashCode([DisallowNull] Tour obj)
        {
            return obj.Id.GetHashCode();
        }
    }

         


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
			ViewBag.Relateds = RelatedTours(tours, tour, id);



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
                Tour = tour,
                Rating=newComment.Rating
                
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
        public IActionResult EditComment(int commentId)
        {
            Comment comment = _context.Comments.Find(commentId);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateComment(int commentId, string text)
        {
            Comment comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
            {
                return NotFound();
            }

            comment.Text = text;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = comment.TourId });
        }

        static List<Tour> RelatedTours(IQueryable<Tour> queryable, Tour tour, int id)
        {
            List<Tour> relateds = new List<Tour>();

            if (tour.CategoryId != 0)
            {
                List<Tour> related = queryable
                    .Include(p => p.Itineraries)
                    .Include(d => d.Comments)
                    .Include(p => p.Destination)
                    .Include(p => p.Category)
                    .Include(p => p.TourDates)
                    .Include(p => p.TourImages)
                    .AsEnumerable()
                    .Where(p => p.CategoryId == tour.CategoryId && p.Id != id && !relateds.Contains(p, new TourComparer()))
                    .ToList();

                relateds.AddRange(related);
            }

            return relateds;
        }


    }
}
