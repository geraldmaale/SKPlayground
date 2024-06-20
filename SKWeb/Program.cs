using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var azureOpenAIApiKey = configuration["AzureOpenAI:ApiKey"];
var azureOpenAIUri = configuration["AzureOpenAI:Uri"];

builder.Services.AddKernel();

builder.Services.AddAzureOpenAIChatCompletion(
    "gpt-35-turbo",
    endpoint: azureOpenAIUri!,
    apiKey: azureOpenAIApiKey!
);

var app = builder.Build();

app.MapGet(
    "/weatherforecast",
    async (Kernel kernel) =>
    {
        var temperature = Random.Shared.Next(-20, 55);
        var summary = await kernel.InvokePromptAsync<string>(
            $"Short description of the weather at {temperature}°C."
        );
        return new WeatherForecast(DateOnly.FromDateTime(DateTime.Now), temperature, summary);
    }
);

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary);
