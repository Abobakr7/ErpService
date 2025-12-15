using ErpService.Data.Entities;
using ErpService.Dtos;
using ErpService.Repositories;
using ErpService.Abstractions;

namespace ErpService.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<InvoiceService> _logger;

        public InvoiceService(IInvoiceRepository invoiceRepository, ILogger<InvoiceService> logger)
        {
            _invoiceRepository = invoiceRepository;
            _logger = logger;
        }

    public async Task<InvoiceResponse?> StoreInvoiceAsync(InvoiceRequest request)
        {
            try
            {
                // buisness validation 
                ValidateBuisness(request);

                // html invoice
                byte[] htmlInvoice;
                using (var ms = new MemoryStream())
                {
                    await request.InvoiceDoc.CopyToAsync(ms);
                    htmlInvoice = ms.ToArray();
                }

                ValidateHtmlFile(request.InvoiceDoc, htmlInvoice);


                var invoice = new Invoice
                {
                    InvoiceId = request.InvoiceId,
                    TicketId = request.TicketId,
                    TicketSerial = request.TicketSerial,
                    From = request.From,
                    To = request.To,
                    NumberOfHours = request.NumberOfHours,
                    LicensePlate = request.LicensePlate,
                    AmountWithoutTax = request.AmountWithoutTax,
                    TaxAmount = request.TaxAmount,
                    Amount = request.Amount,
                    InvoiceHtmlContent = htmlInvoice,
                    CreatedAt = DateTime.UtcNow
                };


                var saved = await _invoiceRepository.SaveInvoiceAsync(invoice);

                if (!saved)
                {
                    _logger.LogWarning("Invoice {InvoiceId} already exists", request.InvoiceId);
                    return null; 
                }

                _logger.LogInformation("Invoice {InvoiceId} successfully saved", request.InvoiceId);

                return new InvoiceResponse
                {
                    InvoiceId = invoice.InvoiceId,
                    TicketId = invoice.TicketId,
                    TicketSerial = invoice.TicketSerial,
                    Amount = invoice.Amount,
                    CreatedAtUtc = invoice.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing invoice {InvoiceId}", request.InvoiceId);
                throw;
            }


        }


        private void ValidateBuisness(InvoiceRequest request)
        {
            if (request.From >= request.To)
            {
                throw new InvalidOperationException("From date must be before To date");
            }

            // number of hours validation
            var duration = request.To - request.From;
            var actualHours = (decimal)duration.TotalHours;

            if (Math.Abs(actualHours - request.NumberOfHours) > 0.5m) //30 mins allowance 
            {
                throw new InvalidOperationException(
                    $"NumberOfHours ({request.NumberOfHours}) doesn't match duration ({actualHours:F2} hours)"
                );
            }

            // total = amount + tax
            if (request.Amount != request.AmountWithoutTax + request.TaxAmount)
            {
                throw new InvalidOperationException(
                    "Amount must equal AmountWithoutTax + TaxAmount"
                );
            }

            // tax
            var expectedTax = Math.Round(request.AmountWithoutTax * 0.15m, 2);
            if (Math.Abs(request.TaxAmount - expectedTax) > 0.01m)
            {
                throw new InvalidOperationException(
                    $"TaxAmount ({request.TaxAmount}) doesn't match 15% VAT ({expectedTax})"
                );
            }
        }

        private void ValidateHtmlFile(IFormFile file, byte[] content)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".html" && extension != ".htm")
            {
                throw new InvalidOperationException("Invoice document must be an HTML file");
            }

            const long maxFileSize = 5 * 1024 * 1024;
            if (content.Length > maxFileSize)
            {
                throw new InvalidOperationException("Invoice HTML file exceeds maximum size of 5MB");
            }

            // empty files handle
            if (content.Length < 100)
            {
                throw new InvalidOperationException("Invoice HTML file is too small or empty");
            }

            // HTML validation
            var htmlText = System.Text.Encoding.UTF8.GetString(content);
            if (!htmlText.Contains("<html", StringComparison.OrdinalIgnoreCase) &&
                !htmlText.Contains("<!DOCTYPE", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Invalid HTML content");
            }
        }
    }
}
