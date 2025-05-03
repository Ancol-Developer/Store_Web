using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShoppingCart.Repository.Validation;

namespace ShoppingCart.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }
		[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập Tên sản phẩm")]
		public string Name { get; set; }
        public string Slug { get; set; }

		[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập Mô tả sản phẩm")]
		public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
		[Required(ErrorMessage = "Chọn một thương hiệu")]
		public int BrandId { get; set; }
		[Required(ErrorMessage = "Chọn một danh mục")]
		public int CategoryId { get; set; }
        public string Image { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual CategoryModel Category { get; set; }
        [ForeignKey(nameof(BrandId))]
        public virtual BrandModel Brand { get; set; }

        public virtual ICollection<OrderDetails> OrderDetails { get; set; }

        public virtual ICollection<RatingModel> Ratings { get; set; }

        [NotMapped]
        [FileExtention]
        public IFormFile? ImageUpload { get; set; }
    }
}
