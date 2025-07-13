using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    [Table("user_profile")]
    public class UserProfile
    {
        public UserProfile(
            string userId,
            string? fullName,
            string? phone,
            string bio,
            DateOnly birthDate,
            string? imageUrl,
            Address address)
        {
            UserId = userId;
            FullName = fullName;
            Phone = phone;
            Bio = bio;
            BirthDate = birthDate;
            ImageUrl = imageUrl;
            Address = address;
        }

        [Key]
        [ForeignKey("User")]
        [Column("user_id")]
        public string UserId { get; init; }

        [Column("full_name")]
        public string? FullName { get; init; }

        [Column("phone")]
        public string? Phone { get; init; }

        [Column("bio")]
        public string Bio { get; init; }

        [Column("birth_date")]
        public DateOnly BirthDate { get; init; }

        [Column("image_url")]
        public string? ImageUrl { get; init; }

        // Embedded (owned) object
        public Address Address { get; init; } = new Address();

        public virtual User User { get; init; } = null!;
    }

    [Owned]
    public class Address
    {
        [Column("street")]
        public string Street { get; init; } = string.Empty;

        [Column("district")]
        public string District { get; init; } = string.Empty;

        [Column("city")]
        public string City { get; init; } = string.Empty;

        [Column("postal_code")]
        public string PostalCode { get; init; } = string.Empty;

        [Column("country")]
        public string Country { get; init; } = string.Empty;
    }
}
