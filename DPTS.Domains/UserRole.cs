using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    [Table("user_role")]
    public class UserRole
    {
        private UserRole() { } // For EF

        public UserRole(string userRoleName, string userRoleDescription)
        {
            UserRoleId = Guid.NewGuid().ToString();
            UserRoleName = userRoleName;
            UserRoleDescription = userRoleDescription;
            Users = new List<User>();
        }

        [Key]
        [Column("user_role_id")]
        public string UserRoleId { get; init; }

        [Required]
        [MaxLength(50)]
        [Column("user_role_name")]
        public string UserRoleName { get; init; }

        [MaxLength(200)]
        [Column("user_role_description")]
        public string UserRoleDescription { get; init; }

        public virtual ICollection<User> Users { get; init; } = new List<User>();
    }
}
