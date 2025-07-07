using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class UserProfile
    {
        [Key]
        [ForeignKey("User")]
        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Column("full_name")]
        public string? FullName { get; set; }

        [Column("phone")]
        public string? Phone { get; set; }
        [Column("bio")]
        public string Bio { get; set; } = string.Empty;
        [Column("birth_date")]
        public DateOnly BirthDate { get; set; }

        [Column("image_url")]
        public string? ImageUrl { get; set; }

        // Embedded object
        public Address Address { get; set; } = new Address();

        public virtual User User { get; set; } = null!;
    }
}
