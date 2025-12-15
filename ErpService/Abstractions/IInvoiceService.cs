using ErpService.Dtos;

namespace ErpService.Abstractions
{
    public interface IInvoiceService
    {
        Task<InvoiceResponse?> StoreInvoiceAsync(InvoiceRequest request);
    }
}
