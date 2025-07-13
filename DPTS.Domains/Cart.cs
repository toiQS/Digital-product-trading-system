using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    [Table("cart")]
    public class Cart
    {
        private Cart() { } // For EF

        public Cart(string buyerId)
        {
            CartId = Guid.NewGuid().ToString();
            BuyerId = buyerId;
            UpdateAt = DateTime.UtcNow;
            CartItems = new List<CartItem>();
        }

        [Key]
        [Column("cart_id")]
        public string CartId { get; init; }

        [Required]
        [Column("buyer_id")]
        public string BuyerId { get; init; }

        [Column("update_at")]
        public DateTime UpdateAt { get; init; }

        public virtual User User { get; init; } = null!;
        public virtual ICollection<CartItem> CartItems { get; init; } = new List<CartItem>();
    }
}
