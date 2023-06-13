using Alps_Hiking.Entities;
using SendGrid.Helpers.Mail;
using System.ComponentModel.DataAnnotations;

namespace Alps_Hiking.ViewModels
{
    public class OrderVM
    {
        [Required]
        [StringLength(maximumLength:50)]
        public string Fullname { get; set; }

        [Required]
        [StringLength(maximumLength:150)]
        public string Adress { get; set; }

        [Required]
        [StringLength(maximumLength:50)]
        public string Username { get; set; }

        [Required]
        [StringLength(maximumLength: 70)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(maximumLength: 150)]
        public string Number { get; set; }
        public decimal TotalPrice { get; set; }
        public List<BasketItem> BasketItems { get; set; }
    }
}
