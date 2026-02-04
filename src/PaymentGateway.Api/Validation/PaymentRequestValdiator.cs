using FluentValidation;

using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Validation;

public class PaymentRequestValidator : AbstractValidator<PaymentRequest>
{
    private static readonly HashSet<string> AllowedCurrencies = ["GBP", "EUR", "USD"];

    public PaymentRequestValidator(TimeProvider timeProvider)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        
        RuleFor(x => x.CardNumber)
            .NotEmpty()
            .Length(14, 19)
            .Must(x => x.All(char.IsDigit)).WithMessage("Card number must contain only numeric characters.");

        RuleFor(x => x.ExpiryMonth)
            .InclusiveBetween(1, 12)
            .DependentRules(() =>
            {
                RuleFor(x => x)
                    .Must(x => IsExpiryInFuture(x, timeProvider))
                    .WithMessage("Card expiry must be in the future.")
                    .WithName("Expiry");
            });

        RuleFor(x => x.ExpiryYear)
            .GreaterThanOrEqualTo(timeProvider.GetUtcNow().Year);

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3)
            .Must(x => AllowedCurrencies.Contains(x))
            .WithMessage($"Currency must be one of: {string.Join(", ", AllowedCurrencies)}");

        RuleFor(x => x.Amount)
            .GreaterThan(0);

        RuleFor(x => x.Cvv)
            .NotEmpty()
            .Length(3, 4)
            .Must(x => x.All(char.IsDigit)).WithMessage("CVV must contain only numeric characters.");
    }

    private static bool IsExpiryInFuture(PaymentRequest request, TimeProvider timeProvider)
    {
        var now = timeProvider.GetUtcNow();
        
        // Get the day of the last month in the expiry month/year
        var expiry = new DateOnly(request.ExpiryYear, request.ExpiryMonth, 1).AddMonths(1).AddDays(-1);
        return expiry >= DateOnly.FromDateTime(now.DateTime);
    }
}