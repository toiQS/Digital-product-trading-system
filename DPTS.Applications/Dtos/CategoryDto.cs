namespace DPTS.Applications.Dtos
{
    internal class CategoryDto
    {
    }
    public class CategoryIndexDto
    {
        public string CategoryId { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryIcon { get; set; } = string.Empty;
        public long Quantity { get; set; } = 0;
    }
}
 