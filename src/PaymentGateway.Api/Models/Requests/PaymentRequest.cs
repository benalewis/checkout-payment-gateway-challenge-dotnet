namespace PaymentGateway.Api.Models.Requests;

using System.Text.Json.Serialization;

public record PaymentRequest(
    [property: JsonPropertyName("card_number")] string CardNumber,
    [property: JsonPropertyName("expiry_month")] int ExpiryMonth,
    [property: JsonPropertyName("expiry_year")] int ExpiryYear,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("amount")] int Amount,
    [property: JsonPropertyName("cvv")] string Cvv
);