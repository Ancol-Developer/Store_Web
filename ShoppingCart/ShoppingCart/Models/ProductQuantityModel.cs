using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingCart.Models
{
    public class ProductQuantityModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Không được bỏ trống số lượng sản phẩm")]
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedDate { get; set; }

        [ForeignKey(nameof(ProductId))]
        public virtual ProductModel Product { get; set; }
    }
}
