using ErpService.Data;
using ErpService.Repositories;
using ErpService.Repositories.Impl;
using ErpService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddScoped<IInvoiceRepository, EFInvoiceRepository>();
builder.Services.AddScoped<InvoiceService>();


builder.Services.AddControllers();

var app = builder.Build();


app.UseHttpsRedirection();

app.MapControllers();

app.Run();
