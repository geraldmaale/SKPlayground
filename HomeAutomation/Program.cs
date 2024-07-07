/*
 Copyright (c) Microsoft. All rights reserved.
*/

using HomeAutomation;
using HomeAutomation.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SKShared.Plugins;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddConsole().SetMinimumLevel(LogLevel.Trace));
builder.Configuration.AddUserSecrets<Program>();

// Actual code to execute is found in Worker class
builder.Services.AddHostedService<Worker>();
        
// Get configuration
builder
    .Services.AddOptions<AzureOpenAI>()
    .Bind(builder.Configuration.GetSection(nameof(AzureOpenAI)))
    .ValidateDataAnnotations()
    .ValidateOnStart();

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

// Add plugins that can be used by kernels
// The plugins are added as singletons so that they can be used by multiple kernels
builder.Services.AddSingleton<MyTimePlugin>();
builder.Services.AddSingleton<MyAlarmPlugin>();
builder.Services.AddSingleton<ThesisWriterPlugin>();
builder.Services.AddKeyedSingleton<MyLightPlugin>("OfficeLight");
builder.Services.AddKeyedSingleton<MyLightPlugin>(
    "PorchLight",
    (sp, key) =>
    {
        return new MyLightPlugin(turnedOn: true);
    }
);

// Add a home automation kernel to the dependency injection container
builder.Services.AddKeyedTransient<Kernel>(
    "HomeAutomationKernel",
    (sp, key) =>
    {
        // Create a collection of plugins that the kernel will use
        KernelPluginCollection pluginCollection = [];
        pluginCollection.AddFromObject(sp.GetRequiredService<MyTimePlugin>());
        pluginCollection.AddFromObject(sp.GetRequiredService<MyAlarmPlugin>());
        pluginCollection.AddFromObject(
            sp.GetRequiredKeyedService<MyLightPlugin>("OfficeLight"),
            "OfficeLight"
        );
        pluginCollection.AddFromObject(
            sp.GetRequiredKeyedService<MyLightPlugin>("PorchLight"),
            "PorchLight"
        );
        pluginCollection.AddFromObject(sp.GetRequiredService<ThesisWriterPlugin>(), "ContentCaching");

        // When created by the dependency injection container, Semantic Kernel logging is included by default
        return new Kernel(sp, pluginCollection);
    }
);

using IHost host = builder.Build();

await host.RunAsync();