using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class Tax
    {
        [Key]
        [Column("tax_id")]
        public string TaxId { get; set; } = string.Empty;
        [Column("tax_name")]
        public string TaxName { get; set; } = string.Empty;
        [Column("tax_description")]
        public string TaxDescription { get; set; } = string.Empty;
        [Column("rate")]
        public decimal Rate { get; set; } = 0.0m;

        [Column("category_id")]
        public string? CategoryId { get; set; } = string.Empty;
        [Column("status")]
        public TaxStatus TaxStatus { get; set; }

        public virtual Category? Category { get; set; }
    }
    public enum TaxStatus
    {
        Active,
        Inactive
    }

}
