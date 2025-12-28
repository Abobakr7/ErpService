using ErpService.Dtos;
using FluentValidation;

namespace ErpService.FluentValidation;

public class InvoiceRequestValidator : AbstractValidator<InvoiceRequest>
{
    public InvoiceRequestValidator()
    {
        RuleFor(i => i.InvoiceId)
            .NotEmpty()
            .WithMessage("Inovice Id is required");
        
        RuleFor(i => i.TicketId)
            .NotEmpty()
            .WithMessage("Ticket Id is required");
        
        RuleFor(i => i.TicketSerial)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Ticket serial number is required")
            .Matches(@"^\d{9}$")
            .WithMessage("Ticket serial number should be 9 digits");
        
        RuleFor(i => i.From)
            .NotEmpty()
            .WithMessage("From datetime is required");
        
        RuleFor(i => i.To)
            .NotEmpty()
            .WithMessage("To datetime is required");
        
        RuleFor(i => i.NumberOfHours)
            .InclusiveBetween(1m, 23m)
            .WithMessage("Number of hours must be between 1 and 23");
        
        RuleFor(i => i.LicensePlate)
            .Matches(@"^[A-Z]{3}\d{1,4}$")
            .WithMessage("Wrong Saudi license plate format");
        
        RuleFor(i => i.TaxAmount)
            .GreaterThanOrEqualTo(0m)
            .WithMessage("TaxAmount cannot be negative");

        RuleFor(i => i.AmountWithoutTax)
            .GreaterThanOrEqualTo(0m)
            .WithMessage("AmountWithoutTax cannot be negative");
        
        RuleFor(i => i.Amount)
            .GreaterThanOrEqualTo(0m)
            .WithMessage("Amount cannot be negative");
        
        RuleFor(i => i.InvoiceDoc)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("Invoice HTML document must be provided")
            .Must(file => file.Length > 0)
            .WithMessage("Invoice HTML document cannot be empty")
            .Must(file => file.Length <= 5 * 1024 * 1024)
            .WithMessage("Invoice HTML file exceeds maximum size of 5MB")
            .Must(file => file.ContentType == "text/html" ||
                        file.FileName.EndsWith(".html", StringComparison.OrdinalIgnoreCase) ||
                        file.FileName.EndsWith(".htm", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Invoice document must be an HTML file");
    }
}
