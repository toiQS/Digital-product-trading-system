using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class Order
    {
        [Key]
        [Column("order_id")]
        public string OrderId { get; set; } = string.Empty;

        [Column("buyer_id")]
        public string BuyerId { get; set; } = string.Empty ;

        [Column("total_amount")]
        public double TotalAmount { get; set; }

        //[Column("status")]
        //public string Status { get; set; } = string.Empty;

        [Column("Is Paied")]
        public bool IsPaied { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual User Buyer { get; set; } = null!;
        public virtual Escrow Escrow { get; set; } = null!;
        public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); 
    }

}
