using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    [Table("order_item")]
    public class OrderItem
    {
        private OrderItem() { } // For EF

        public OrderItem(string orderId, string productId, int quantity, decimal priceForEachProduct)
        {
            OrderItemId = Guid.NewGuid().ToString();
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
            PriceForEachProduct = priceForEachProduct;
            TotalPrice = quantity * priceForEachProduct;
        }

        [Key]
        [Column("order_item_id")]
        public string OrderItemId { get; init; }

        [Required]
        [Column("order_id")]
        public string OrderId { get; init; }

        [Required]
        [Column("product_id")]
        public string ProductId { get; init; }

        [Column("quantity")]
        public int Quantity { get; init; } = 1;

        [Column("price_for_each_product")]
        public decimal PriceForEachProduct { get; init; }

        [Column("total_price")]
        public decimal TotalPrice { get; init; }

        // Reserved field for applied discount or tax in future
        [Column("adjusted_amount")]
        public decimal? AdjustedAmount { get; init; }

        public virtual Order Order { get; init; } = null!;
        public virtual Product Product { get; init; } = null!;
    }
}
