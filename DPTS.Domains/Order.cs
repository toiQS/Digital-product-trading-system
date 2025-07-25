﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class Order
    {
        [Key]
        [Column("order_id")]
        public string OrderId { get; set; } = string.Empty;

        [Required]
        [Column("buyer_id")]
        public string BuyerId { get; set; } = string.Empty;

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Column("is_paid")]
        public bool IsPaid { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual User Buyer { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<Escrow> Escrows { get; set; } = new List<Escrow>();
    }
}
