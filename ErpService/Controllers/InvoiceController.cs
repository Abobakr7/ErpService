using ErpService.Dtos;
using ErpService.Services;
using Microsoft.AspNetCore.Mvc;
using ErpService.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Diagnostics.HealthChecks;


namespace ErpService.Controllers
{
    [ApiController]
    [Route("/api/invoices")]
    [Authorize]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceService invoiceService;
        private readonly ILogger<InvoiceController> _logger;
        public InvoiceController(InvoiceService invoiceService, ILogger<InvoiceController> logger)
        {
            this.invoiceService = invoiceService;
            _logger = logger;
        }


        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<InvoiceResponse>> StoreInvoice([FromForm] InvoiceRequest request)
        {
            try
            {
                _logger.LogInformation("Receiving invoice submission for InvoiceId: {InvoiceId}",
                    request.InvoiceId);

                var response = await invoiceService.StoreInvoiceAsync(request);

                if (response == null)
                {
                    // Invoice already exists
                    _logger.LogWarning("Duplicate invoice attempt: {InvoiceId}", request.InvoiceId);
                    return Conflict(new ErrorResponse
                    {
                        Message = $"Invoice with ID {request.InvoiceId} already exists",
                        InvoiceId = request.InvoiceId
                    });
                }

                _logger.LogInformation("Invoice {InvoiceId} created successfully", response.InvoiceId);

                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business validation failed for invoice {InvoiceId}",
                    request.InvoiceId);

                return BadRequest(new ErrorResponse
                {
                    Message = ex.Message,
                    InvoiceId = request.InvoiceId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing invoice {InvoiceId}", request.InvoiceId);

                return StatusCode(500, new ErrorResponse
                {
                    Message = "An error occurred while processing the invoice",
                    InvoiceId = request.InvoiceId
                });
            }
        }


        [HttpGet("Check")]
        public string Check()
        {
            return "Invoice Service is up.";
        }
    }
}