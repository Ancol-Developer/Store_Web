using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Models
{
    public class CouponModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="Yêu cầu nhập tên coupon")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập mô tả coupon")]

        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập giá")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập số lượng")]
        public int Quantity { get; set; }
        public int Status { get; set; }
    }
}
