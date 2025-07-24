namespace DPTS.Applications.Admin.manage_order.dtos
{
    public class OrderDto
    {
        public int OrderCount { get; set; }
        public List<OrderIndexDto> Indexs { get; set; }
    }
    public class OrderIndexDto
    {
        public string EscrowId { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public decimal TotalPrice{ get; set; }
        public string Status { get; set; }
    }
}
