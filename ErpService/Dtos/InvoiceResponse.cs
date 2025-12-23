namespace ErpService.Dtos
{
    public sealed class InvoiceResponse
    {
        public Guid InvoiceId { get; init; }
        public Guid TicketId { get; init; }
        public string TicketSerial { get; init; } = string.Empty;
        public decimal Amount { get; init; }
        public decimal AmountAfterTax { get; init; }
        public decimal TaxAmount { get; init; }
        public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
    }
}
