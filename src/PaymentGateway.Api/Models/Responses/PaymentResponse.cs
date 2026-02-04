using PaymentGateway.Api.Enums;

namespace PaymentGateway.Api.Models.Responses;

public record PaymentResponse(
    Guid Id,
    PaymentStatus Status,
    string CardNumberLastFour,
    int ExpiryMonth,
    int ExpiryYear,
    string Currency,
    int Amount
);