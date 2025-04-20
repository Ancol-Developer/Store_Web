using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Repository.Validation
{
    public class FileExtentionAttribute : ValidationAttribute
    {
        public FileExtentionAttribute()
        {
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var ext = Path.GetExtension(file.FileName);
                string[] extention = { ".jpg", ".jpeg", ".png" };

                bool result = extention.Any(x => ext.EndsWith(x));
                if (!result)
                {
                    return new ValidationResult($"Allowed extentions are jpg, jpeg, png");
                }
            }
            return ValidationResult.Success;
        }
    }
}
