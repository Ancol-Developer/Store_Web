using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Models.ViewModel
{
    public class ProductDetailViewModel
    {
        public ProductModel Product { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập đánh giá")]
        public string Comment { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập email")]
        public string Email { get; set; }
    }
}
