using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    [Table("user_role")]
    public class UserRole
    {
        public UserRole(string userRoleName, string userRoleDescription)
        {
            UserRoleId = Guid.NewGuid().ToString();
            UserRoleName = userRoleName;
            UserRoleDescription = userRoleDescription;
        }

        [Key, Column("user_role_id")]
        public string UserRoleId { get; init; }

        [Column("user_role_name")]
        public string UserRoleName { get; init; }

        [Column("user_role_description")]
        public string UserRoleDescription { get; init; }
        private UserRole() { }
        
    }
}
