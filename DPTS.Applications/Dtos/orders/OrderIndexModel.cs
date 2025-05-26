using System.Text;

namespace DPTS.Applications.Dtos.orders
{
    public class OrderIndexModel
    {
       public string OrderId { get; set; } = string.Empty;
        public string BuyerName { get; set; } = string.Empty;
        public StringBuilder ProductionName { get; set; } = new StringBuilder();
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
