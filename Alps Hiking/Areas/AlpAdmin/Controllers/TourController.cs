using Alps_Hiking.DAL;
using Alps_Hiking.Entities;
using Alps_Hiking.Utilities.Extensions;
using Alps_Hiking.Utilities.Roles;
using Alps_Hiking.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Composition;

namespace Alps_Hiking.Areas.AlpAdmin.Controllers
{
    [Area("AlpAdmin")]
    [Authorize(Roles = "Admin")]
    public class TourController : Controller
    {
        private readonly AlpsHikingDbContext _context;
        private readonly IWebHostEnvironment _env;

        public TourController(AlpsHikingDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1)
        {
            ViewBag.TotalPage = Math.Ceiling((double)_context.Tours.Count() / 16);
            ViewBag.CurrentPage = page;
            ViewBag.Tours = _context.Tours
                               .AsNoTracking().Skip((page - 1) * 16).Take(16).AsEnumerable();
            IEnumerable<Tour> tours = _context.Tours.Include(t => t.TourImages)
                    .Include(t => t.TourDates)
                        .Include(t => t.PassengerCounts)
                            .Include(t => t.Itineraries)
                                .Include(t => t.Destination)
                                    .Include(t => t.Comments)
                                        .Include(t => t.Category).AsNoTracking().Skip((page - 1) * 16).Take(16).AsEnumerable();
            return View(tours);
        }

        public IActionResult Delete(int id)
        {
            if (id == 0) return NotFound();
            Tour? tour = _context.Tours
                .Include(t => t.TourImages)
                   .Include(t => t.TourDates)
                       .Include(t => t.PassengerCounts)
                           .Include(t => t.Itineraries)
                               .Include(t => t.Destination)
                                   .Include(t => t.Comments)
                                       .Include(t => t.Category).FirstOrDefault(c => c.Id == id);
            if (tour is null) return NotFound();
            return View(tour);
        }




        public IActionResult Create()
        {
            ViewBag.Category = _context.Categories.AsEnumerable();
            ViewBag.Itinerary = _context.Itineraries.AsEnumerable();
            ViewBag.PassengerCount = _context.PassengerCounts.AsEnumerable();
            ViewBag.Tourdate = _context.TourDates.AsEnumerable();
            ViewBag.Destination = _context.Destiantions.AsEnumerable();
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Create(TourVm newVm)
        {
            ViewBag.Category = _context.Categories.AsEnumerable();
            ViewBag.Itinerary = _context.Itineraries.AsEnumerable();
            ViewBag.PassengerCount = _context.PassengerCounts.AsEnumerable();
            ViewBag.Tourdate = _context.TourDates.AsEnumerable();
            ViewBag.Destination = _context.Destiantions.AsEnumerable();
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (!newVm.MainPhoto.IsValidFile("image/"))
            {
                ModelState.AddModelError(string.Empty, "Please choose image file");
                return View();
            }
            if (!newVm.MainPhoto.IsValidLength(1))
            {
                ModelState.AddModelError(string.Empty, "Please choose image which size is maximum 1MB");
                return View();
            }
            newVm.DiscountPrice = newVm.Price - (newVm.Price * newVm.Discount / 100);
            Tour tour = new()
            {
                Name = newVm.Name,
                DayCount = newVm.DayCount,
                Difficulty = newVm.Difficulty,
                Pickup = newVm.Pickup,
                DiscountPrice = newVm.DiscountPrice,
                Discount = newVm.Discount,
                PassangerAge = newVm.PassangerAge,
                CategoryId = newVm.CategoryId,
                Description = newVm.Description,
                Airport = newVm.Airport,
                MeetingPoint = newVm.MeetingPoint,
                Expect=newVm.Expect,
                Map=newVm.Map,  
                Price= newVm.Price,
                DestinationId=newVm.DestinationId,
            };

            var imagefolderPath = Path.Combine(_env.WebRootPath, "assets", "images");
        


            TourImage main = new()
            {
                IsMain = true,
                Path = await newVm.MainPhoto.CreateImage(imagefolderPath, "toursphotos")
            };
            tour.TourImages.Add(main);
            foreach (var image in newVm.HoverPhoto)
            {
                if (!image.IsValidFile("image/") || !image.IsValidLength(1))
                {
                    return View();
                }
                TourImage hover = new()
                {
                    IsMain = false,
                    Path = await image.CreateImage(imagefolderPath, "toursphotos")
                };
                tour.TourImages.Add(hover);
            }

            foreach (int id in newVm.Itineraries)
            {
                Itinerary It=_context.Itineraries.FirstOrDefault(x => x.Id == id);  
                Itinerary itinerary = new()
                {
                    TourId = id,
                    Tour=tour,
                    Day=It.Day,
                    Day_count=It.Day_count, 
                };
                tour.Itineraries.Add(itinerary);
            }

            foreach (var item in newVm.TourDates)
            {
                TourDate tours = _context.TourDates.FirstOrDefault(x => x.Id == item);
                TourDate tourDate = new()
                {
                    TourId = item,
                    MaxPassengerCount=tours.MaxPassengerCount,
                   TourDates=tours.TourDates,
                };
                tour.TourDates.Add(tourDate);
            }
            _context.Tours.Add(tour);
            _context.SaveChanges();
            return RedirectToAction("Index", "Tour");
        }

    }
}
