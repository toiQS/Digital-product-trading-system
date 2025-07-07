using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class OrderItem
    {
        [Key]
        [Column("order_item_id")]
        public string OrderItemId { get; set; } = string.Empty;

        [Required]
        [Column("product_id")]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        [Column("quantity")]
        public int Quantity { get; set; } = 1;

        [Column("price_for_each_product")]
        public decimal PriceForeachProduct { get; set; }

        [Column("total_price")]
        public decimal TotalPrice { get; set; } 

        [Required]
        [Column("order_id")]
        public string OrderId { get; set; } = string.Empty;

        public virtual Product Product { get; set; } = null!;
        public virtual Order Order { get; set; } = null!;
    }
}