using System.Net;

using OneOf;

namespace PaymentGateway.Api.Bank;

public interface IBankHttpClient
{
    public Task<OneOf<BankResponse, BankError>> ProcessPayment(BankRequest request, CancellationToken ct);
}

public class BankHttpClient(HttpClient httpClient) : IBankHttpClient
{
    public async Task<OneOf<BankResponse, BankError>> ProcessPayment(BankRequest request, CancellationToken ct)
    {
        var response = await httpClient.PostAsJsonAsync("/payments", request, ct);
        return response.StatusCode switch
        {
            HttpStatusCode.BadRequest => (await response.Content.ReadFromJsonAsync<BankError>(ct))!,
            HttpStatusCode.OK => (await response.Content.ReadFromJsonAsync<BankResponse>(ct))!,
            HttpStatusCode.ServiceUnavailable => new BankError("Bank is unavailable"),
            _ => new BankError($"Unexpected status: {response.StatusCode}")
        };
    }
}