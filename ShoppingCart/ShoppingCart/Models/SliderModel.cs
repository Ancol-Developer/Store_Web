using System.ComponentModel.DataAnnotations;
using ShoppingCart.Repository.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingCart.Models
{
    public class SliderModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Hãy nhập tên Slider")]
        public string Name { get; set; }
        [Required(ErrorMessage ="Hãy nhập mô tả Slider")]
        public string Description { get; set; }
        public int Status { get; set; }
        public string Image {  get; set; }
        [NotMapped]
        [FileExtention]
        public IFormFile ImageUpload { get; set; }
    }
}
