namespace ShoppingCart.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public string Username { get; set; }
        public decimal ShippingCost { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }
    }
}
