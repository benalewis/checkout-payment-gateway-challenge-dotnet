using System.Net;
using JustEat.HttpClientInterception;
using PaymentGateway.Api.Bank.Models;

namespace PaymentGateway.Api.Tests;

public static class BankSimulator
{
    public static void SetupOkResponse(this HttpClientInterceptorOptions options, string authCode = "test-auth-code",
        bool authorised = true)
    {
        new HttpRequestInterceptionBuilder()
            .ForPost()
            .ForHttps()
            .ForHost("bank-simulator")
            .ForPath("payments")
            .WithStatus(HttpStatusCode.OK)
            .WithJsonContent(new BankResponse(authorised, authCode))
            .RegisterWith(options);
    }

    public static void SetupBadRequest(this HttpClientInterceptorOptions options, string error = "Invalid request")
    {
        new HttpRequestInterceptionBuilder()
            .ForPost()
            .ForHttps()
            .ForHost("bank-simulator")
            .ForPath("payments")
            .WithStatus(HttpStatusCode.BadRequest)
            .WithJsonContent(new BankError(error))
            .RegisterWith(options);
    }

    public static void SetupServiceUnavailable(this HttpClientInterceptorOptions options)
    {
        new HttpRequestInterceptionBuilder()
            .ForPost()
            .ForHttps()
            .ForHost("bank-simulator")
            .ForPath("payments")
            .WithStatus(HttpStatusCode.ServiceUnavailable)
            .RegisterWith(options);
    }
}