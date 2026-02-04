namespace PaymentGateway.Api.Bank;

public class BankHttpClientOptions
{
    public const string SectionName = "BankHttpClient";
    public required string BaseUrl { get; init; }
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(5);
}