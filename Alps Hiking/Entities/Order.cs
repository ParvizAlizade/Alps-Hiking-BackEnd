using Alps_Hiking.Utilities;
using Alps_Hiking.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace Alps_Hiking.Entities
{
    public class Order:BaseEntity
    {
        [StringLength(maximumLength: 250)]
		public string Address { get; set; }

		[Required]
		[StringLength(maximumLength: 50)]
		public string FullName { get; set; }

		[Required]
		[StringLength(maximumLength: 100)]
		public string Email { get; set; }

		public DateTime CreatedAt { get; set; }
		public OrderStatus Status { get; set; }
		public decimal TotalPrice { get; set; }
		public string Number { get; set; }
		public List<OrderItem> OrderItems { get; set; }
		public string UserId { get; set; }
        public User User { get; set; }
    }
}
