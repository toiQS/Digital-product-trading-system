using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class EscrowProcess
    {
        [Key]
        [Column("process_id")]
        public string ProcessId { get; set; } = string.Empty;
        [Column("process_name")]
        public string ProcessName { get; set; } = string.Empty;
        [Column("process_at")]
        public DateTime ProcessAt { get; set; }
        [Column("escrow_id")]
        public string EscrowId { get; set; } = string.Empty;
        [Column("escrow_process_information")]
        public string EscrowProcessInformation {  get; set; } = string.Empty;
        public Escrow Escrow { get; set; } = null!;
    }
}
