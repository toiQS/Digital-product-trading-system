using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class Store
    {
        [Key]
        [Column(name:"store_id")]
        public string StoreId {  get; set; } = string.Empty;
        [Column("store_name")]
        public string StoreName { get; set; } = string.Empty ;
        [Column("create_at")]
        public DateTime CreateAt { get; set; }
        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;
        [Column("status")]
        public StoreStatus Status { get; set; }
        [Column("store_image")]
        public string StoreImage { get; set; } = string.Empty;

        // fk
        public virtual User User { get; set; } = null!;
    }
    public enum StoreStatus
    {
        Active,
        Inactive,
        Unknown,
    }
}
