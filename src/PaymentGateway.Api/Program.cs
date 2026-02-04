using System.Text.Json;
using System.Text.Json.Serialization;

using FluentValidation;

using Microsoft.Extensions.Options;

using PaymentGateway.Api.Bank;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Validation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddValidatorsFromAssemblyContaining<PaymentRequestValidator>();

builder.Services.AddSingleton<PaymentsRepository>();
builder.Services.AddScoped<PaymentsService>();

builder.Services.Configure<BankHttpClientOptions>(
    builder.Configuration.GetSection(BankHttpClientOptions.SectionName));

builder.Services.AddHttpClient<BankHttpClient>((sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<BankHttpClientOptions>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
    client.Timeout = settings.Timeout;
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();