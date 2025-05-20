using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sys.Domain
{
    public class User
    {
        [Key]
        [Column("Id")]
        public string Id { get; set; } = string.Empty;
        [Column("Role Id")]
        //[ForeignKey(nameof)]
        public string RoleId { get; set; } = string.Empty;
        [Column("User Name")]
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        [Column("Password Hash")]
        public string PasswordHash { get; set; } = string.Empty;
        [Column("Full Name")]
        public string FullName { get; set; } = string.Empty;
        [Column("Number Phone")]
        public string NumberPhone {  get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        [Column("Two Factor Secret")]
        public string TwoFactorSecret {  get; set; } = string.Empty;
        [Column("Two Factor Enabled")]
        public bool TwoFactorEnabled { get; set; }
        [Column("Create At")]
        public DateTime CreateAt { get; set; }
        [Column("Update At")]
        public DateTime UpdateAt { get; set; }


        //ForeignKey
        public virtual Role Role { get; set; } = new Role();
    }
}
