namespace ErpService.Dtos
{
    public sealed class ErrorResponse
    {
        public string Message { get; init; } = default!;
        public Guid InvoiceId { get; init; }
    }
}