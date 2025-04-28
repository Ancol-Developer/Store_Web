using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Models.ViewModel
{
	public class UserViewModel
	{
		public int Id { get; set; }
		[Required(ErrorMessage ="Vui lòng nhập UserName")]
		public string Username { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập Password"), DataType(DataType.Password)]
		public string Password { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập Email"), EmailAddress(ErrorMessage ="Nhập đúng định dạng email")]
		public string Email { get; set; }
	}
}
