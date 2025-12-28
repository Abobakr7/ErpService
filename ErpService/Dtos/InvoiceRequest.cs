namespace ErpService.Dtos
{
    public sealed class InvoiceRequest
    {
        public Guid InvoiceId { get; init; }
        public Guid TicketId { get; init; }
        public required string TicketSerial { get; init; }
        public DateTime From { get; init; }
        public DateTime To { get; init; }
        public decimal NumberOfHours { get; init; }
        public required string LicensePlate { get; init; }
        public decimal AmountWithoutTax { get; init; }
        public decimal TaxAmount { get; init; }
        public decimal Amount { get; init; }
        public required IFormFile InvoiceDoc { get; init; }
    }
}
