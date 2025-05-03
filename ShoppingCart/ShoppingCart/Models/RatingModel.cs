using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingCart.Models
{
    public class RatingModel
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập đánh giá")]
        public string Comment { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập email")]
        public string Email { get; set; }

        public string Star { get; set; }

        [ForeignKey("ProductId")]
        public virtual ProductModel Product { get; set; }
    }
}
