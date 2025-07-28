namespace DPTS.Applications.Buyer.Dtos.product
{
    public class ProductIndexListDto
    {
        public int TotalCount { get; set; }
        public List<ProductIndexDto> ProductIndexList { get; set; } = new List<ProductIndexDto>();

    }
}
