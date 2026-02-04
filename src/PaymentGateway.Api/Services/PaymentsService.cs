using FluentValidation;

using PaymentGateway.Api.Bank;
using PaymentGateway.Api.Bank.Models;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

using OneOf;

using PaymentGateway.Api.Controllers;

namespace PaymentGateway.Api.Services;

public class PaymentsService(
    IValidator<PaymentRequest> validator,
    BankHttpClient bankClient,
    PaymentsRepository repository)
{
    public PaymentResponse? GetById(Guid id) => repository.Get(id);

    public async Task<OneOf<PaymentResponse, DependencyFailure>> ProcessAsync(
        PaymentRequest request, CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(request, ct);

        if (!validationResult.IsValid)
            return CreateResponse(request, PaymentStatus.Rejected);

        var bankRequest = new BankRequest(
            request.CardNumber,
            $"{request.ExpiryMonth:D2}/{request.ExpiryYear}",
            request.Currency.ToUpperInvariant(),
            request.Amount,
            request.Cvv
        );

        var bankResponse = await bankClient.ProcessPaymentAsync(bankRequest, ct);
        return bankResponse.Match<OneOf<PaymentResponse, DependencyFailure>>(
            success => SaveAndReturn(request,
                success.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined),
            _ => SaveAndReturn(request, PaymentStatus.Rejected),
            failure => failure
        );

        PaymentResponse SaveAndReturn(PaymentRequest req, PaymentStatus status)
        {
            var response = CreateResponse(req, status);
            repository.Add(response);
            return response;
        }
    }

    private static PaymentResponse CreateResponse(PaymentRequest request, PaymentStatus status) =>
        new(
            Id: Guid.NewGuid(),
            Status: status,
            CardNumberLastFour: request.CardNumber[^4..],
            ExpiryMonth: request.ExpiryMonth,
            ExpiryYear: request.ExpiryYear,
            Currency: request.Currency.ToUpperInvariant(),
            Amount: request.Amount
        );
}