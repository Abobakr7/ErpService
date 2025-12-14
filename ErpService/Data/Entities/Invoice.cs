using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErpService.Data.Entities
{
    [Table("Invoices")]
    public class Invoice
    {
        public Guid InvoiceId { get; set; }
        public Guid TicketId { get; set; }
        public string TicketSerial { get; set; } = null!;
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public decimal NumberOfHours { get; set; }
        public string LicensePlate { get; set; } = null!;
        public decimal AmountWithoutTax { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Amount { get; set; }
        public byte[] InvoiceHtmlContent { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
