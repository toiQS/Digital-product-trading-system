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
        public Store(string storeName, DateTime createAt, string userId, StoreStatus status, string storeImage)
        {
            StoreId = Guid.NewGuid().ToString();
            StoreName = storeName;
            CreateAt = createAt;
            UserId = userId;
            Status = status;
            StoreImage = storeImage;
        }

        [Key]
        [Column("store_id")]
        public string StoreId { get; init; }

        [Column("store_name")]
        public string StoreName { get; init; }

        [Column("create_at")]
        public DateTime CreateAt { get; init; }

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
    }
}
