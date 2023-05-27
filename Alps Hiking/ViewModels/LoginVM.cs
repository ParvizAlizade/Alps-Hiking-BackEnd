using System.ComponentModel.DataAnnotations;

namespace Alps_Hiking.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Enter The Username")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Enter The Password")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
