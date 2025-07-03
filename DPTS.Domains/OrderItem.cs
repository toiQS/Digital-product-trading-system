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

        [Required]
        [Column("original_price")]
        public decimal OriginalPrice { get; set; }

        [Column("discount_amount")]
        public decimal DiscountAmount { get; set; } = 0;

        [Column("tax_amount")]
        public decimal TaxAmount { get; set; } = 0;

        [Column("platform_fee_amount")]
        public decimal PlatformFeeAmount { get; set; } = 0;

        [Column("final_price")]
        public decimal FinalPrice { get; set; }

        [Required]
        [Column("order_id")]
        public string OrderId { get; set; } = string.Empty;

        public virtual Product Product { get; set; } = null!;
        public virtual Order Order { get; set; } = null!;
    }
}