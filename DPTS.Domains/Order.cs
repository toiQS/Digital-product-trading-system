using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public enum OrderStatus
    {
        Unknown,
        Pending,           // Đang xử lý (mới tạo)
        WaitingConfirm,    // Người bán cần xác nhận
        BuyerConfirmed,    // Người mua đã xác nhận nhận hàng
        Done,              // Giao dịch hoàn tất
        Complaint,         // Khiếu nại
        Canceled,          // Đã huỷ
        Failed             // Thanh toán thất bại, hoặc xử lý lỗi
    }

    [Table("order")]
    public class Order
    {
        private Order() { }

        public Order(string buyerId, decimal totalAmount, bool isPaid, OrderStatus status)
        {
            OrderId = Guid.NewGuid().ToString();
            BuyerId = buyerId;
            TotalAmount = totalAmount;
            IsPaid = isPaid;
            Status = status;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            OrderItems = new List<OrderItem>();
            Processes = new List<OrderProcess>();
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
        public bool IsPaid { get; set; }

        [Column("status")]
        public OrderStatus Status { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; init; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        public virtual User Buyer { get; init; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; init; }
        public virtual ICollection<OrderProcess> Processes { get; init; }
        public virtual Escrow Escrow { get; init; } = null!;
    }
}
