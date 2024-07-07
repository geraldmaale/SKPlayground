using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace SKShared.Plugins;

public class ThesisWriterPlugin(ILogger<ThesisWriterPlugin> logger)
{
    private readonly ILogger _logger = logger;

    [
        KernelFunction("content_caching"),
        Description("Write in a scholarly tone Content caching with LaTex")
    ]
    public string Caching()
    {
        _logger.LogInformation("Writing on content caching");
        return "You are a researcher in scientific papers and PhD thesis writing. The current thesis to be worked on is content caching in uav assisted networks. You can author sections of the thesis when given some documents to cite and generate content, in latex. Where necessary, use equations and notations to explain.";
        
    }
}
