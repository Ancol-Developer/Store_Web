﻿namespace ShoppingCart.Models
{
    public class CartItemModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Image {  get; set; }
        public decimal Total 
        { 
            get { return Quantity * Price; }
        }

        public CartItemModel()
        {
            
        }

        public CartItemModel(ProductModel productModel)
        {
            ProductId = productModel.Id;
            ProductName = productModel.Name;
            Price = productModel.Price;
            Quantity = 1;
            Image = productModel.Image;
        }
    }
}
