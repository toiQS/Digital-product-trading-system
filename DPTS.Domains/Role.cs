using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class Role
    {
        [Key]
        [Column("role_id")]
        public string RoleId { get; set; } = string.Empty;  

        [Column("role_name")]
        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }

}
