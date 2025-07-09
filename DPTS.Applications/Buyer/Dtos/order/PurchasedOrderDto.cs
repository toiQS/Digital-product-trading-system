namespace DPTS.Applications.Buyer.Dtos.order
{
    public class PurchasedOrderDto
    {
        public List<OrderIndexDto> OrderIndexDtos { get; set; } = new List<OrderIndexDto>();
        public List<OverViewIndexDto> OrverViewIndexDtos { get;set; } = new List<OverViewIndexDto>();
    }
}
