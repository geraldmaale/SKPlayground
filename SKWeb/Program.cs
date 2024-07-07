using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SKShared.Options;

var builder = WebApplication.CreateBuilder(args);

// Get configuration
builder
    .Services.AddOptions<AzureOpenAI>()
    .Bind(builder.Configuration.GetSection(nameof(AzureOpenAI)))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddKernel();

// Chat completion service that kernels will use
builder.Services.AddSingleton<IChatCompletionService>(sp =>
{
    AzureOpenAI options = sp.GetRequiredService<IOptions<AzureOpenAI>>().Value;

    // A custom HttpClient can be provided to this constructor
    return new AzureOpenAIChatCompletionService(
        options.ChatDeploymentName,
        options.Endpoint,
        options.ApiKey
    );
});

var app = builder.Build();

app.MapGet(
    "/weatherforecast",
    async (Kernel kernel) =>
    {
        var temperature = Random.Shared.Next(-20, 55);
        var summary = await kernel.InvokePromptAsync<string>(
            $"Very short description of the weather at {temperature}°C."
        );
        return new WeatherForecast(DateOnly.FromDateTime(DateTime.Now), temperature, summary);
    }
);

app.MapPost("/turn-light-off", () =>
{
    
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary);
