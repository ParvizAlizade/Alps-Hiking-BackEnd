using System.ComponentModel.DataAnnotations;

namespace Alps_Hiking.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Please Enter The FirstName")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Please Enter The LastName")]
        public string Lastname { get; set; }

        [StringLength(maximumLength: 15)]
        public string Username { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Click Here")]
        public bool Terms { get; set; }
    }
}
