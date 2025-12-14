using ErpService.Dtos;
using ErpService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ErpService.Controllers
{
    [ApiController]
    [Route("/api/invoices")]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceService invoiceService;

        public InvoiceController(InvoiceService invoiceService)
        {
            this.invoiceService = invoiceService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<InvoiceResponse>> StoreInvoice([FromForm] InvoiceRequest request)
        {
            InvoiceResponse? response = await invoiceService.StoreInvoiceAsync(request);

            if (response == null)
                return Conflict(new ErrorResponse { InvoiceId = request.InvoiceId, Message = "Invoice with this Id already exist." });

            return StatusCode(StatusCodes.Status201Created, response);
        }
    }
}
