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
        public TourController(AlpsHikingDbContext context, UserManager<User> userManager)
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


            if (destinationId != 0)
            {
                alltour = alltour.Where(x => x.DestinationId == destinationId);
            }

            List<Tour> tours = alltour.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Destination = _context.Destiantions.Include(x => x.Tours).ToList();
            ViewBag.Tours = _context.Tours.ToList();

            ViewBag.Slider = _context.Sliders.ToList();
            return View(tours);
        }

        [HttpPost]
        public IActionResult Index(int[] categoirId, int[] days, double minprice, double maxprice, byte difficulty, int rating, int[] age, int[] destiantions)
        {
            IQueryable<Tour> alltour = _context.Tours
                .Include(t => t.Category)
                    .Include(t => t.TourDates)
                       .Include(t => t.TourImages)
                         .Include(t => t.Itineraries)
                           .Include(t => t.PassengerCounts)
                             .Include(t => t.Destination);


            if (categoirId.Length > 0)
            {
                alltour = alltour.Where(x => categoirId.Contains(x.CategoryId));
            }
            if (days.Length > 0)
            {
                alltour = alltour.Where(x => days.Contains(x.DayCount));
            }
            if (minprice != 0 || maxprice != 0)
            {
                alltour = alltour.Where(x => minprice < x.DiscountPrice && x.DiscountPrice < maxprice);
            }

            if (difficulty != 0)
            {
                alltour = alltour.Where(x => difficulty == (x.Difficulty));
            }
            if (rating != 0)
            {
                alltour = alltour.Where(x => rating == (x.Rate));
            }
            if (age.Length > 0)
            {
                alltour = alltour.Where(x => age.Contains(x.PassangerAge));
            }
            if (destiantions.Length > 0)
            {
                alltour = alltour.Where(x => destiantions.Contains(x.DestinationId));
            }
            List<Tour> tours = alltour.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Destination = _context.Destiantions.Include(x => x.Tours).ToList();
            ViewBag.Tours = _context.Tours.ToList();
            ViewBag.Slider = _context.Sliders.ToList();
            return View(tours);
        }

        public IActionResult Details(int id)
        {
            if (id == 0) return NotFound();
            IQueryable<Tour> tours = _context.Tours.AsNoTracking().AsQueryable();
            Tour? tour = tours.Include(d => d.TourImages)
                .Include(d => d.Category)
                .Include(d => d.Destination)
                .Include(d => d.PassengerCounts)
                .Include(d => d.TourDates)
                .Include(d => d.TourImages)
                .Include(d => d.Comments).ThenInclude(u => u.User)
                .Include(d => d.Itineraries)
                                     .AsSingleQuery().FirstOrDefault(d => d.Id == id);

            ViewBag.Comment = _context.Comments.ToList();
            ViewBag.Relateds = RelatedTours(tours, tour, id);
            if (tour is null) return NotFound();
            return View(tour);
        }

        public async Task<IActionResult> RemoveBasketItem(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                User user = await _userManager.FindByNameAsync(User.Identity.Name);
                BasketItem? basketItem = _context.BasketItems.Include(t => t.TourDate).FirstOrDefault(b => b.Id == id && b.UserId == user.Id);

                if (basketItem != null)
                {
                    _context.BasketItems.Remove(basketItem);
                    _context.SaveChanges();
                }
            }

            return Redirect(Request.Headers["Referer"].ToString());
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
                Rating = newComment.Rating

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

        [HttpPost]
        public async Task<IActionResult> Details(int id, int count, int dateid)
        {
            if (id == 0) return NotFound();

            IQueryable<Tour> tours = _context.Tours.AsNoTracking().AsQueryable();
            Tour? tour = tours.Include(d => d.TourImages)
          .Include(d => d.Category)
          .Include(d => d.Destination)
          .Include(d => d.PassengerCounts)
          .Include(d => d.TourDates)
          .Include(d => d.TourImages)
          .Include(d => d.Comments).ThenInclude(u => u.User)
          .Include(d => d.Itineraries)
                               .AsSingleQuery().FirstOrDefault(d => d.Id == id);
            if (User.Identity.IsAuthenticated == false)
                return RedirectToAction("login", "account");

            if (User.Identity.IsAuthenticated)
            {

                User user = await _userManager.FindByNameAsync(User.Identity.Name);
                BasketItem basketItem = _context.BasketItems.FirstOrDefault(b => b.TourDateId == dateid && b.UserId == user.Id);
                if (basketItem == null)
                {
                    basketItem = new BasketItem()
                    {
                        UserId = user.Id,
                        TourDateId = dateid,
                        Count = count,
                        Price= (decimal)tour.DiscountPrice
                    };
                    _context.BasketItems.Add(basketItem);
                }
                else
                {
                    basketItem.Count+=count;
                }
                _context.SaveChanges();


                ViewBag.Comment = _context.Comments.ToList();
                ViewBag.Relateds = RelatedTours(tours, tour, id);
                return Redirect(Request.Headers["Referer"].ToString());
            }
            ViewBag.Comment = _context.Comments.ToList();
            ViewBag.Relateds = RelatedTours(tours, tour, id);
            return Redirect(Request.Headers["Referer"].ToString());
        }






        public async Task<IActionResult> AddToWishList(int tourid)
        {
            Tour tour = await _context.Tours.FindAsync(tourid);

            if (tour is null)
            {
                return NotFound();
            }

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            User user = await _userManager.FindByNameAsync(User.Identity.Name);

            WishListItem userWishlistItem = await _context.wishListItems
                .FirstOrDefaultAsync(x => x.UserId == user.Id && x.TourId == tourid);

            if (userWishlistItem is null)
            {
                userWishlistItem = new WishListItem
                {
                    UserId = user.Id,
                    TourId = tourid
                };
                _context.wishListItems.Add(userWishlistItem);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> RemoveFromWishList(int wishListItemId)
        {
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            WishListItem wishListItem = await _context.wishListItems
                .FirstOrDefaultAsync(x => x.UserId == user.Id && x.Id == wishListItemId);
            if (wishListItem is null)
            {
                return NotFound();
            }
            _context.wishListItems.Remove(wishListItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(WishList));
        }

        public async Task<IActionResult> WishList()
        {

			ViewBag.Slider = _context.Sliders.ToList();
			if (!User.Identity.IsAuthenticated)
            {
                return View(new List<WishListItem>());
            }

            var userId = _userManager.GetUserId(User);

            var wishListItems = _context.wishListItems
                .Include(wli => wli.Tour)
                .ThenInclude(p => p.TourImages)
                .Where(wli => wli.UserId == userId)
                .ToList();

            if (wishListItems.Count == 0)
            {
                return View(new List<WishListItem>());
            }

            return View(wishListItems);
        }
    }
}



