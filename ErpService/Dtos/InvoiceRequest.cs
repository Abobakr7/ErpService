using ErpService.ValidationAnnotations;
using System.ComponentModel.DataAnnotations;

namespace ErpService.Dtos
{
    public sealed class InvoiceRequest
    {
        [NotEmptyGuid]
        public Guid InvoiceId { get; init; }

        [NotEmptyGuid]
        public Guid TicketId { get; init; }

        [Required]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "TicketSerial must be exactly 9 digits.")]
        public string TicketSerial { get; init; } = default!;

        [NotDefaultDate]
        public DateTime From { get; init; }

        [NotDefaultDate]
        public DateTime To { get; init; }

        [Range(1, 23)]
        public decimal NumberOfHours { get; init; }

        [Required]
        [RegularExpression(
            @"^[0-9]{4}[A-Z]{3}$",
            ErrorMessage = "Invalid Saudi license plate format."
        )]
        public string LicensePlate { get; init; } = default!;

        [Range(0, 9999)]
        [DataType(DataType.Currency)]
        public decimal AmountWithoutTax { get; init; }

        [Range(0, 9999)]
        [DataType(DataType.Currency)]
        public decimal TaxAmount { get; init; }

        [Range(0, 9999)]
        [DataType(DataType.Currency)]
        public decimal Amount { get; init; }

        [Required] // html file
        public IFormFile InvoiceDoc { get; init; } = default!;
    }
}
