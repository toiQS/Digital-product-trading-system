using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sys.Domain
{
    public class Role
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Column("Role Name")]
        public string RoleName { get; set; } = string.Empty;
        public string RoleDescription { get; set; } = string.Empty ;
    }
}
