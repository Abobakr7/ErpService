using ErpService.Data;
using ErpService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ErpService.Repositories.Impl
{
    public class EFInvoiceRepository : IInvoiceRepository
    {
        private readonly AppDbContext context;

        public EFInvoiceRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> SaveInvoiceAsync(Invoice invoice)
        {
            var exists = await context.Invoices.AnyAsync(i => i.InvoiceId == invoice.InvoiceId);

            if (exists)
                return false;

            await context.Invoices.AddAsync(invoice);

            await context.SaveChangesAsync();
            
            return true;
        }
    }
}
