using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class Role
    {
        [Key]
        [Column("role_id")]
        public string RoleId { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Column("role_name")]
        public string RoleName { get; set; } = string.Empty;

        [Column("description")]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
