namespace PaymentGateway.Api.Models.Responses;

using System.Text.Json.Serialization;
using Enums;

public record PaymentResponse(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("status")] PaymentStatus Status,
    [property: JsonPropertyName("card_number_last_four")] string CardNumberLastFour,
    [property: JsonPropertyName("expiry_month")] int ExpiryMonth,
    [property: JsonPropertyName("expiry_year")] int ExpiryYear,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("amount")] int Amount
);