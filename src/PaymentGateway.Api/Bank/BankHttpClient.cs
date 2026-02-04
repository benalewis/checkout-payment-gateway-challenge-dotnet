using System.Net;

using OneOf;

using PaymentGateway.Api.Bank.Models;
using PaymentGateway.Api.Controllers;

namespace PaymentGateway.Api.Bank;

public class BankHttpClient(HttpClient httpClient)
{
    public async Task<OneOf<BankResponse, BankError, DependencyFailure>> ProcessPaymentAsync(BankRequest request, CancellationToken ct)
    {
        var response = await httpClient.PostAsJsonAsync("/payments", request, ct);
        return response.StatusCode switch
        {
            HttpStatusCode.OK => (await response.Content.ReadFromJsonAsync<BankResponse>(ct))!,
            HttpStatusCode.BadRequest => (await response.Content.ReadFromJsonAsync<BankError>(ct))!,
            HttpStatusCode.ServiceUnavailable => new DependencyFailure("Bank", "Bank is unavailable"),
            _ => new DependencyFailure("Bank", $"Bank returned unexpected status: {response.StatusCode}")
        };
    }
}