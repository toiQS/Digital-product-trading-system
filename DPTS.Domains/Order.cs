using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    [Table("order")]
    public class Order
    {
        private Order() { } // For EF

        public Order(string buyerId, decimal totalAmount, bool isPaid, OrderStatus status)
        {
            OrderId = Guid.NewGuid().ToString();
            BuyerId = buyerId;
            TotalAmount = totalAmount;
            IsPaid = isPaid;
            Status = status;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            Processs = new List<OrderProcess>();
            OrderItems = new List<OrderItem>();
        }

        [Key]
        [Column("order_id")]
        public string OrderId { get; init; }

        [Required]
        [Column("buyer_id")]
        public string BuyerId { get; init; }

        [Column("total_amount")]
        public decimal TotalAmount { get; init; }

        [Column("is_paid")]
        public bool IsPaid { get; init; }

        [Column("status")]
        public OrderStatus Status { get; init; }

        [Column("created_at")]
        public DateTime CreatedAt { get; init; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; init; }

        public virtual User Buyer { get; init; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; init; } = new List<OrderItem>();
        public virtual Escrow Escrow { get; init; } = null!;
        public virtual ICollection<OrderProcess> Processs { get; init; } = new List<OrderProcess>();
    }

    public enum OrderStatus
    {
        Unknown,           // Không rõ
        Pending,           // Đang xử lý
        WaitingConfirm,    // Chờ xác nhận
        BuyerConfirmed,    // Đã xác nhận
        Done,              // Hoàn tất
        Complaint,         // Khiếu nại
        Canceled,          // Đã huỷ
        Failed             // Lỗi
    }
}
