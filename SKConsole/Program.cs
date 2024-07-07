using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel.Plugins.Web.Bing;

var configBuilder = new ConfigurationBuilder()
    .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .Build();

var azureOpenAIApiKey = configBuilder["AzureOpenAI:ApiKey"];
var azureOpenAIUri = configBuilder["AzureOpenAI:Endpoint"];
var bingSearchApiKey = configBuilder["BingSearch:ApiKey"];
var azureOpenAIDeploymentName = configBuilder["AzureOpenAI:ChatDeploymentName"];

var builder = Kernel.CreateBuilder();

builder.Services.AddLogging(loggingBuilder =>
    loggingBuilder.AddConsole().SetMinimumLevel(LogLevel.Trace)
);

builder.Services.ConfigureHttpClientDefaults(httpClientBuilder =>
{
    httpClientBuilder.AddStandardResilienceHandler();
    httpClientBuilder.RedactLoggedHeaders(["Authorization", "api-key"]);
});
builder.Services.AddRedaction();

var kernel = builder
    .AddAzureOpenAIChatCompletion(azureOpenAIDeploymentName!, endpoint: azureOpenAIUri!, apiKey: azureOpenAIApiKey!)
    .Build();

//Use plugins
kernel.ImportPluginFromType<Demograhpics>();

// Suppress the SKEXP0050 diagnostic
#pragma warning disable SKEXP0050

// Import the WebSearchEnginePlugin with BingConnector
kernel.ImportPluginFromObject(new WebSearchEnginePlugin(new BingConnector(bingSearchApiKey!)));

var promptSettings = new OpenAIPromptExecutionSettings()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

// Get the chat service
var chatService = kernel.GetRequiredService<IChatCompletionService>();
ChatHistory chat = new();

while (true)
{
    Console.Write("Q: ");
    var prompt = Console.ReadLine();
    if (prompt == "exit" || string.IsNullOrEmpty(prompt))
    {
        break;
    }
    chat.AddUserMessage(prompt);
    var response = await chatService.GetChatMessageContentAsync(chat, promptSettings, kernel);
    Console.WriteLine($"A: {response}");
    chat.Add(response);
}

class Demograhpics
{
    [KernelFunction]
    public int GetPersonAge(string name)
    {
        return name switch
        {
            "Gerald" => 30,
            "Agnes" => 10,
            _ => 5
        };
    }
}
