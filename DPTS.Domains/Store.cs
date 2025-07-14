using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public enum StoreStatus
    {
        Active,
        Inactive,
        Unknown,
    }

    [Table("store")]
    public class Store
    {
        private Store() { } // For EF

        public Store(string storeName, string userId, string storeImage, StoreStatus status = StoreStatus.Active)
        {
            StoreId = Guid.NewGuid().ToString();
            StoreName = storeName;
            UserId = userId;
            StoreImage = storeImage;
            Status = status;
            CreateAt = DateTime.UtcNow;
        }

        [Key]
        [Column("store_id")]
        public string StoreId { get; init; }

        [Required]
        [Column("store_name")]
        public string StoreName { get; init; }

        [Required]
        [Column("create_at")]
        public DateTime CreateAt { get; init; }

        [Required]
        [Column("user_id")]
        public string UserId { get; init; }

        [Column("status")]
        public StoreStatus Status { get; init; }

        [Column("store_image")]
        public string StoreImage { get; init; }

        // Navigation properties
        public virtual User User { get; init; } = null!;
        public virtual ICollection<Message> SentMessages { get; init; } = new List<Message>();
        public virtual ICollection<Message> ReceivedMessages { get; init; } = new List<Message>();
        public virtual ICollection<Product> Products { get; init; } = new List<Product>();
        public virtual ICollection<Escrow> Escrows { get; init; } = new List<Escrow>();
    }
}
