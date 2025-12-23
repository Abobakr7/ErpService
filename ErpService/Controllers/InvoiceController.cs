using ErpService.Dtos;
using ErpService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ErpService.Controllers
{
    [ApiController]
    [Route("/api/invoices")]
     //[Authorize] // Disabled for testing
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceService invoiceService;

        public InvoiceController(InvoiceService invoiceService)
        {
            this.invoiceService = invoiceService;
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult<InvoiceResponse>> StoreInvoice([FromBody] InvoiceRequest request)
        {
            try
            {
                var response = await invoiceService.StoreInvoiceAsync(request);

                if (response == null)
                {
                    return Conflict(new ErrorResponse
                    {
                        Message = $"Invoice with ID {request.InvoiceId} already exists",
                        InvoiceId = request.InvoiceId
                    });
                }
                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = ex.Message,
                    InvoiceId = request.InvoiceId
                });
            }
            catch (Exception ex)
            {
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