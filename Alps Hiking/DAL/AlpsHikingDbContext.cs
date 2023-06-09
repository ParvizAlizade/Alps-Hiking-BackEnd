using Alps_Hiking.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Alps_Hiking.DAL
{
	public class AlpsHikingDbContext : IdentityDbContext<User>
	{
        public AlpsHikingDbContext(DbContextOptions<AlpsHikingDbContext> options) : base(options)
		{

		}

		public DbSet<Setting> Settings { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Destiantion> Destiantions { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourImage> TourImages { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Itinerary> Itineraries { get; set; }
        public DbSet<PassengerCount>PassengerCounts { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TourDate> TourDates { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Profession> Profession { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<BasketItem> BasketItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Setting>()
				.HasIndex(s => s.Key)
				.IsUnique();
            base.OnModelCreating(modelBuilder);

        }
    }
}
