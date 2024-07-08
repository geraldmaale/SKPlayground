// Copyright (c) Microsoft. All rights reserved.

using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace SKShared.Plugins;

/// <summary>
/// Simple plugin that just returns the time.
/// </summary>
public class MyTimePlugin(ILogger<MyTimePlugin> logger)
{
    private readonly ILogger _logger = logger;
    
    [KernelFunction, Description("Get the current time")]
    public DateTimeOffset Time() => DateTimeOffset.Now;

    [KernelFunction, Description("Events in 2024")]
    public List<string> Events2024()
    {
        _logger.LogInformation( "Fetching events in 2024" );
        return
        [
            "Christmas",
            "New Year",
            "Agnes' Birthday",
            "Schola's birthday",
            "Graduation day is 2024/11/25"
        ];
    }
}
