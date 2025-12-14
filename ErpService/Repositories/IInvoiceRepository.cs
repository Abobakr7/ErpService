using ErpService.Data.Entities;

namespace ErpService.Repositories
{
    public interface IInvoiceRepository
    {
        Task<bool> SaveInvoiceAsync(Invoice invoice);
    }
}
