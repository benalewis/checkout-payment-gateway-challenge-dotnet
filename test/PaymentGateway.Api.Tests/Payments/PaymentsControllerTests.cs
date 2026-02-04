using System.Net;
using System.Net.Http.Json;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Tests;

public class PaymentsControllerTests : IClassFixture<PaymentGatewayWebApplicationFactory>, IDisposable
{
    private readonly HttpClient _client;
    private readonly PaymentGatewayWebApplicationFactory _factory;

    public PaymentsControllerTests(PaymentGatewayWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.Interceptor.Clear();
        _client = factory.CreateClient();
    }

    public void Dispose() => _factory.Interceptor.Clear();

    [Fact]
    public async Task ProcessPayment_WhenBankAuthorizes_ReturnsAuthorized()
    {
        // Arrange
        _factory.Interceptor.SetupOkResponse();
        var request = CreateValidRequest();

        // Act
        var response = await _client.PostAsJsonAsync("/api/payments", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payment = await response.Content.ReadFromJsonAsync<PaymentResponse>();
        Assert.Equal(PaymentStatus.Authorized, payment!.Status);
    }

    [Fact]
    public async Task ProcessPayment_WhenBankDeclines_ReturnsDeclined()
    {
        // Arrange
        _factory.Interceptor.SetupOkResponse(authorised: false);
        var request = CreateValidRequest();

        // Act
        var response = await _client.PostAsJsonAsync("/api/payments", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payment = await response.Content.ReadFromJsonAsync<PaymentResponse>();
        Assert.Equal(PaymentStatus.Declined, payment!.Status);
    }

    [Fact]
    public async Task ProcessPayment_WhenBankUnavailable_Returns500()
    {
        // Arrange
        _factory.Interceptor.SetupServiceUnavailable();
        var request = CreateValidRequest();

        // Act
        var response = await _client.PostAsJsonAsync("/api/payments", request);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    private static PaymentRequest CreateValidRequest() => new(
        CardNumber: "2222405343248877",
        ExpiryMonth: 12,
        ExpiryYear: DateTime.UtcNow.Year + 1,
        Currency: "GBP",
        Amount: 1050,
        Cvv: "123"
    );
    
    [Fact]
    public async Task GetPayment_WhenPaymentExists_ReturnsPayment()
    {
        // Arrange
        _factory.Interceptor.SetupOkResponse();
        var request = CreateValidRequest();
        var createResponse = await _client.PostAsJsonAsync("/api/payments", request);
        var createdPayment = await createResponse.Content.ReadFromJsonAsync<PaymentResponse>();
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

        // Act
        var response = await _client.GetAsync($"/api/payments/{createdPayment!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payment = await response.Content.ReadFromJsonAsync<PaymentResponse>();
        Assert.NotNull(payment);
        Assert.Equal(createdPayment.Id, payment!.Id);
        Assert.Equal(createdPayment.Status, payment.Status);
        Assert.Equal(createdPayment.CardNumberLastFour, payment.CardNumberLastFour);
        Assert.Equal(createdPayment.Amount, payment.Amount);
        Assert.Equal(createdPayment.Currency, payment.Currency);
    }

    [Fact]
    public async Task GetPayment_WhenPaymentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/payments/{nonExistentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}