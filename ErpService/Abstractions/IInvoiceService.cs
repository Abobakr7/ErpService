using ErpService.Data.Entities;
using ErpService.Dtos;

namespace ErpService.Abstractions
{
    public interface IInvoiceService
    {
        Task<InvoiceResponse?> StoreInvoiceAsync(InvoiceRequest request);
        Task<List<InvoiceResponse>> GetInvoices();
        Task<string> GenerateInvoicesDashboardHtml();
    }
}
