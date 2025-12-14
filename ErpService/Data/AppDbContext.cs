using ErpService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ErpService.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Invoice> Invoices { get; set; } = null!;

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("Invoices");

                entity.HasKey(e => e.InvoiceId);
                entity.Property(e => e.InvoiceId)
                      .ValueGeneratedNever();

                entity.Property(e => e.TicketId)
                      .IsRequired();

                entity.Property(e => e.TicketSerial)
                      .IsRequired()
                      .HasColumnType("char(9)")
                      .HasMaxLength(9);

                entity.Property(e => e.From)
                      .IsRequired();

                entity.Property(e => e.To)
                      .IsRequired();

                entity.Property(e => e.NumberOfHours)
                      .HasColumnType("decimal(4,2)")
                      .HasPrecision(4, 2);

                entity.Property(e => e.LicensePlate)
                      .IsRequired()
                      .HasMaxLength(7);

                entity.Property(e => e.AmountWithoutTax)
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.TaxAmount)
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Amount)
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.InvoiceHtmlContent)
                      .IsRequired();

                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("SYSUTCDATETIME()"); // UTC default
            });
        }
    }
}
