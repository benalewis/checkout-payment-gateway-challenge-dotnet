using Microsoft.Extensions.Configuration;

namespace PaymentGateway.Api.Tests;

using JustEat.HttpClientInterception;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

public class PaymentGatewayWebApplicationFactory : WebApplicationFactory<Program>
{
    internal HttpClientInterceptorOptions Interceptor { get; } = new()
    {
        ThrowOnMissingRegistration = true
    };

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["BankHttpClient:BaseUrl"] = "https://bank-simulator"
            });
        });
        
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<IHttpMessageHandlerBuilderFilter>(
                new InterceptionFilter(Interceptor));
        });
    }

    private sealed class InterceptionFilter(HttpClientInterceptorOptions options) 
        : IHttpMessageHandlerBuilderFilter
    {
        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        {
            return builder =>
            {
                next(builder);
                builder.AdditionalHandlers.Add(options.CreateHttpMessageHandler());
            };
        }
    }
}