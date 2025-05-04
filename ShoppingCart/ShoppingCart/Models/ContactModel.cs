using System.ComponentModel.DataAnnotations;
using ShoppingCart.Repository.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingCart.Models
{
    public class ContactModel
    {
        [Key]
        [Required(ErrorMessage = "Yêu cầu nhập Tiêu đề Website")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập Địa chỉ")]
        public string Map { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập Email"), DataType(DataType.EmailAddress, ErrorMessage = "Nhập đúng kiểu email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập thông tin liên hệ")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập Điện thoại")]
        public string Phone { get; set; }
        public string LogoImage { get; set; }

        [NotMapped]
        [FileExtention]
        public IFormFile ImageUpload { get; set; }
    }
}
