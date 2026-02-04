using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(
    PaymentsService paymentsService) : Controller
{
    [HttpGet("{id:guid}")]
    public Task<ActionResult<PaymentResponse?>> GetPaymentAsync(Guid id, CancellationToken _)
    {
        var payment = paymentsService.GetByIdAsync(id);
        return Task.FromResult<ActionResult<PaymentResponse?>>(new OkObjectResult(payment));
    }
    
    [HttpPost]
    [ProducesResponseType<PaymentResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<PaymentResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<DependencyFailure>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ProcessPayment(
        [FromBody] PaymentRequest request,
        CancellationToken ct)
    {
        var response = await paymentsService.ProcessAsync(request, ct);
        return response.Match<IActionResult>(
            payment => payment.Status == PaymentStatus.Rejected ? BadRequest(payment) : Ok(payment),
            failure => StatusCode(500, failure)
        );
    }
}