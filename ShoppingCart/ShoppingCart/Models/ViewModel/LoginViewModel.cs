using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Models.ViewModel
{
    public class LoginViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập Username")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Password")]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }   
    }
}
