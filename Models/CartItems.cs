namespace Lab3_ShoppingCart.Models
{
    public class CartItems
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public decimal? UnitPrice { get; set; }

        public int quantity { get; set; }
    }
}
