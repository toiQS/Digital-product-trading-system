using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    [Table("order_process")]
    public class OrderProcess
    {
        private OrderProcess() { }

        public OrderProcess(string orderId, string processName, string orderProcessInformation)
        {
            ProcessId = Guid.NewGuid().ToString();
            OrderId = orderId;
            ProcessName = processName;
            OrderProcessInformation = orderProcessInformation;
            ProcessAt = DateTime.UtcNow;
        }

        [Key]
        [Column("process_id")]
        public string ProcessId { get; init; }

        [Required]
        [Column("order_id")]
        public string OrderId { get; init; }

        [Required]
        [Column("process_name")]
        public string ProcessName { get; init; }

        [Column("order_process_information")]
        public string OrderProcessInformation { get; init; }

        [Column("process_at")]
        public DateTime ProcessAt { get; init; }

        public virtual Order Order { get; init; } = null!;
    }
}
