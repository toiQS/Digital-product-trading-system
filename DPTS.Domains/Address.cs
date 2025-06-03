using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class Address
    {
        [Key]
        [Column("address_id")] 
        public string AddressId { get; set; } = string.Empty;
        [Column("street")]
        public string Street { get; set; } = string.Empty;
        [Column("district")]
        public string District { get; set; } = string.Empty;
        [Column("city")]
        public string City { get; set; } = string.Empty;
        [Column("postal_code")]
        public string PostalCode { get; set; } = string.Empty;
        [Column("country")]
        public string Country { get; set; } = string.Empty;
    }
}