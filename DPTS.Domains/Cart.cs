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
            UpdatedAt = DateTime.UtcNow;
            CartItems = new List<CartItem>();
        }

        [Key]
        [Column("cart_id")]
        public string CartId { get; init; }

        [Required]
        [Column("buyer_id")]
        public string BuyerId { get; init; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [Column("is_checked_out")]
        public bool IsCheckedOut { get; set; } = false;

        public virtual User User { get; init; } = null!;
        public virtual ICollection<CartItem> CartItems { get; init; }

        public void Touch()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkCheckedOut()
        {
            IsCheckedOut = true;
        }
        public decimal GetCartTotal()
        {
            return CartItems.Sum(item => item.GetTotalPrice());
        }

    }
}
