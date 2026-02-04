using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Bank;

public record BankError(
    [property: JsonPropertyName("error_message")] string ErrorMessage
);