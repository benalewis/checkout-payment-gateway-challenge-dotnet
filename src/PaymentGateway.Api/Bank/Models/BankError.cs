using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Bank.Models;

public record BankError(
    [property: JsonPropertyName("error_message")] string ErrorMessage
);