namespace ErpService.Dtos
{
    public sealed class InvoiceResponse
    {
        public Guid InvoiceId { get; init; }
        public Guid TicketId { get; init; }
        public string TicketSerial { get; init; } = default!;
        public decimal Amount { get; init; }
        public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
    }
}
