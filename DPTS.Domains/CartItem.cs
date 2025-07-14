using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    [Table("cart_item")]
    public class CartItem
    {
        private CartItem() { }

        public CartItem(string cartId, string productId, int quantity, decimal priceForEachProduct)
        {
            CartItemId = Guid.NewGuid().ToString();
            CartId = cartId;
            ProductId = productId;
            Quantity = quantity;
            PriceForEachProduct = priceForEachProduct;
        }

        [Key]
        [Column("cart_item_id")]
        public string CartItemId { get; init; }

        [Required]
        [Column("cart_id")]
        public string CartId { get; init; }

        [Required]
        [Column("product_id")]
        public string ProductId { get; init; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("price_foreach_product")]
        public decimal PriceForEachProduct { get; set; }

        public virtual Cart Cart { get; init; } = null!;
        public virtual Product Product { get; init; } = null!;

        public void IncreaseQuantity(int amount) => Quantity += amount;
        public void UpdateQuantity(int newQuantity) => Quantity = newQuantity;
        public decimal GetTotalPrice() => Quantity * PriceForEachProduct;
    }

}
