using ErpService.Data.Entities;
using ErpService.Dtos;
using ErpService.Repositories;
using ErpService.Abstractions;
using System.Text;
using Microsoft.EntityFrameworkCore;

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
                
                string htmlContent;
                using (var reader = new StreamReader(request.InvoiceDoc.OpenReadStream()))
                {
                    htmlContent = await reader.ReadToEndAsync();
                }
                
                ValidateHtmlFile(htmlContent);

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
                    InvoiceHtmlContent = htmlContent,
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
                    TaxAmount = invoice.TaxAmount,
                    AmountAfterTax = invoice.AmountWithoutTax,
                    CreatedAtUtc = invoice.CreatedAt,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing invoice {InvoiceId}", request.InvoiceId);
                throw;
            }
        }

        public async Task<List<InvoiceResponse>> GetInvoices()
        {
            return await _invoiceRepository
                            .GetAll()
                            .Select(i => new InvoiceResponse
                            {
                                InvoiceId = i.InvoiceId,
                                TicketId = i.TicketId,
                                TicketSerial = i.TicketSerial,
                                TaxAmount = i.TaxAmount,
                                AmountAfterTax = i.Amount,
                                Amount = i.AmountWithoutTax,
                                CreatedAtUtc = i.CreatedAt
                            })
                            .ToListAsync() ?? [];
        }

        public async Task<string> GenerateInvoicesDashboardHtml()
        {
            var invoices = await _invoiceRepository
                            .GetAll()
                            .ToListAsync() ?? [];

            var totalAmount = invoices.Sum(i => i.Amount);
            var totalTaxAmount = invoices.Sum(i => i.TaxAmount);
            var totalHours = invoices.Sum(i => i.NumberOfHours);
            var totalInvoices = invoices.Count;

            var html = new StringBuilder();
            html.Append("<html><head><title>Invoices Dashboard</title><style>");
            html.Append("body { font-family: Arial, sans-serif; margin: 20px; }");
            html.Append("h1 { color: #333; }");
            html.Append("table { width: 100%; border-collapse: collapse; margin-top: 20px; }");
            html.Append("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
            html.Append("th { background-color: #4CAF50; color: white; }");
            html.Append("tr:nth-child(even) { background-color: #f9f9f9; }");
            html.Append(".stats { margin-top: 20px; font-size: 1.1em; }");
            html.Append(".stats span { display: block; margin-bottom: 5px; }");
            html.Append("</style></head><body>");

            html.Append("<h1>Invoices Dashboard</h1>");

            html.Append("<div class='stats'>");
            html.AppendFormat("<span><strong>Total Invoices:</strong> {0}</span>", totalInvoices);
            html.AppendFormat("<span><strong>Total Amount:</strong> {0}</span>", totalAmount);
            html.AppendFormat("<span><strong>Total Tax Amount:</strong> {0}</span>", totalTaxAmount);
            html.AppendFormat("<span><strong>Total Hours:</strong> {0:F2}</span>", totalHours);
            html.Append("</div>");

            html.Append("<table><thead><tr><th>Invoice ID</th><th>Ticket Serial</th><th>Amount</th><th>Tax Amount</th><th>Total Amount</th><th>Created At</th></tr></thead><tbody>");

            foreach (var invoice in invoices)
            {
                html.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5:yyyy-MM-dd HH:mm:ss}</td></tr>",
                    invoice.InvoiceId, invoice.TicketSerial, invoice.AmountWithoutTax, invoice.TaxAmount, invoice.Amount, invoice.CreatedAt);
            }

            html.Append("</tbody></table></body></html>");

            return html.ToString();
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

            // total = amount - tax
            if (request.Amount != request.AmountWithoutTax - request.TaxAmount)
            {
                throw new InvalidOperationException(
                    "Amount must equal AmountWithoutTax - TaxAmount"
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

        private void ValidateHtmlFile(string content)
        {
            if (!content.Contains("<html", StringComparison.OrdinalIgnoreCase) &&
                !content.Contains("<!DOCTYPE", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Invalid HTML content");
            }
        }
    }
}
