using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Bank;

public record BankResponse(
    [property: JsonPropertyName("authorized")] bool Authorized,
    [property: JsonPropertyName("authorization_code")] string? AuthorizationCode
);