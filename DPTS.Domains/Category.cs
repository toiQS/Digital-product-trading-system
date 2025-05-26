using System.ComponentModel.DataAnnotations;

namespace DPTS.Domains
{
    public class Category
    {
        [Key]
        public string CategoryId {  get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public long Quantity { get; set; }
    }
}
