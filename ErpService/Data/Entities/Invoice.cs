using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErpService.Data.Entities
{
    public class Invoice
    {
        [Key]
        public Guid InvoiceId { get; set; }

        public Guid TicketId { get; set; }

        [MaxLength(9)]
        public string TicketSerial { get; set; } = null!;

        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public decimal NumberOfHours { get; set; }

        [MaxLength(7)]
        public string LicensePlate { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountWithoutTax { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [MaxLength(500)]
        public string HtmlPath { get; set; } = null!;
    }
}
