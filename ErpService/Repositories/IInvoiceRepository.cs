using ErpService.Data.Entities;
using ErpService.Dtos;

namespace ErpService.Repositories
{
    public interface IInvoiceRepository
    {
        Task<bool> SaveInvoiceAsync(Invoice invoice);
        IQueryable<Invoice> GetAll();
    }
}
