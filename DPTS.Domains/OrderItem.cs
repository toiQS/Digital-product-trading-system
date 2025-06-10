using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class OrderItem
    {
        [Key]
        [Column("order_item_id")]
        public string OrderItemId { get; set; } = string.Empty;
        [Column("product_id")]
        public string ProductId { get; set; } = string.Empty;

        [Column("quantity")]
        public int Quantity { get; set; } = 1;

        [Column("total_amount")]
        public double TotalAmount { get; set; }

        [Column("order_id")]
        public string OrderId { get; set; } = string.Empty;

        public virtual Product Product { get; set; } = null!;
        public virtual Order Order { get; set; } = null!;
    }
}
