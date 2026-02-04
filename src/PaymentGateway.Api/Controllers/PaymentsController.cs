using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(
    PaymentsRepository paymentsRepository,
    PaymentService paymentService) : Controller
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostPaymentResponse?>> GetPaymentAsync(Guid id, CancellationToken _)
    {
        var payment = paymentsRepository.Get(id);

        return new OkObjectResult(payment);
    }
    
    [HttpPost]
    [ProducesResponseType<PostPaymentResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<PostPaymentResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ProcessPayment(
        [FromBody] PaymentRequest request,
        CancellationToken ct)
    {
        var response = await paymentService.(request, ct);

        return response.Status == PaymentStatus.Rejected
            ? BadRequest(response)
            : Ok(response);
    }
}