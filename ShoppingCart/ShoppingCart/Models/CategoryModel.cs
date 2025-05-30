﻿using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Models
{
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập Tên danh mục")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập Mô tả danh mục")]
        public string Description { get; set; }
        public string Slug { get; set; }
        public int Status { get; set; }

        public virtual ICollection<ProductModel> Products { get; set; }
    }
}
