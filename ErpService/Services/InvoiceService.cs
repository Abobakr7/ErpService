using ErpService.Data.Entities;
using ErpService.Dtos;
using ErpService.Repositories;

namespace ErpService.Services
{
    public class InvoiceService
    {
        private readonly IInvoiceRepository invoiceRepository;

        public InvoiceService(IInvoiceRepository invoiceRepository)
        {
            this.invoiceRepository = invoiceRepository;
        }

        public async Task<InvoiceResponse?> StoreInvoiceAsync(InvoiceRequest request)
        {
            byte[] htmlContent;
            using (var ms = new MemoryStream())
            {
                await request.InvoiceDoc.CopyToAsync(ms);
                htmlContent = ms.ToArray();
            }

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

            var saved = await invoiceRepository.SaveInvoiceAsync(invoice);

            if (!saved)
                return null;

            return new InvoiceResponse
            {
                InvoiceId = invoice.InvoiceId,
                TicketId = invoice.TicketId,
                TicketSerial = invoice.TicketSerial,
                CreatedAtUtc = invoice.CreatedAt
            };
        }
    }
}
