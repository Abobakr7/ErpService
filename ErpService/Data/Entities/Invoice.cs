using System.ComponentModel.DataAnnotations.Schema;

namespace ErpService.Data.Entities
{
    [Table("Invoices")]
    public class Invoice
    {
        public Guid InvoiceId { get; set; }
        public Guid TicketId { get; set; }
        public required string TicketSerial { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public decimal NumberOfHours { get; set; }
        public required string LicensePlate { get; set; }
        public decimal AmountWithoutTax { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Amount { get; set; }
        public required string InvoiceHtmlContent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
