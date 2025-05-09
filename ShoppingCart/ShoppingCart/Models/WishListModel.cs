using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Models
{
    public class WishListModel
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public ProductModel Product { get; set; }
    }
}
