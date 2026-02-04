using System.Net;
using System.Net.Http.Json;

using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Tests.Payments;

public class PaymentsControllerValidationTests(PaymentGatewayWebApplicationFactory factory) : IClassFixture<PaymentGatewayWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Theory]
    [InlineData("1234567890123")]      // Too short (13)
    [InlineData("12345678901234567890")] // Too long (20)
    [InlineData("1234567890123A")]      // Non-numeric
    public async Task ProcessPayment_WithInvalidCardNumber_ReturnsBadRequest(string cardNumber)
    {
        var request = CreateValidRequest() with { CardNumber = cardNumber };
        var response = await _client.PostAsJsonAsync("/api/payments", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public async Task ProcessPayment_WithInvalidExpiryMonth_ReturnsBadRequest(int month)
    {
        var request = CreateValidRequest() with { ExpiryMonth = month };
        var response = await _client.PostAsJsonAsync("/api/payments", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ProcessPayment_WithExpiredCard_ReturnsBadRequest()
    {
        var request = CreateValidRequest() with { ExpiryYear = 2020, ExpiryMonth = 1 };
        var response = await _client.PostAsJsonAsync("/api/payments", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("GB")]
    [InlineData("XXX")]
    public async Task ProcessPayment_WithInvalidCurrency_ReturnsBadRequest(string currency)
    {
        var request = CreateValidRequest() with { Currency = currency };
        var response = await _client.PostAsJsonAsync("/api/payments", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ProcessPayment_WithInvalidAmount_ReturnsBadRequest(int amount)
    {
        var request = CreateValidRequest() with { Amount = amount };
        var response = await _client.PostAsJsonAsync("/api/payments", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("12")]
    [InlineData("12345")]
    [InlineData("12A")]
    public async Task ProcessPayment_WithInvalidCvv_ReturnsBadRequest(string cvv)
    {
        var request = CreateValidRequest() with { Cvv = cvv };
        var response = await _client.PostAsJsonAsync("/api/payments", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static PaymentRequest CreateValidRequest() => new(
        CardNumber: "2222405343248877",
        ExpiryMonth: 5,
        ExpiryYear: DateTime.UtcNow.Year + 1,
        Currency: "GBP",
        Amount: 1050,
        Cvv: "123"
    );
}