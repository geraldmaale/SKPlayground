using System.Text.Json.Serialization;

namespace AzureAI;


public class ChatResponse
{
    [JsonPropertyName("filePath")]
    public string FilePath { get; set; } = string.Empty;

    [JsonPropertyName("result")]
    public Result? Result { get; set; }
}

public class Result
{
    [JsonPropertyName("choices")]
    public Choice[] Choices { get; set; } = [];

    [JsonPropertyName("created")]
    public long Created { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("object")]
    public string Object { get; set; } = string.Empty;

    [JsonPropertyName("prompt_filter_results")]
    public PromptFilterResult[]? PromptFilterResults { get; set; }

    [JsonPropertyName("system_fingerprint")]
    public string SystemFingerprint { get; set; } = string.Empty;

    [JsonPropertyName("usage")]
    public Usage? Usage { get; set; }
}

public class Choice
{
    [JsonPropertyName("content_filter_results")]
    public ContentFilterResults? ContentFilterResults { get; set; }

    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; set; } = string.Empty;

    [JsonPropertyName("index")]
    public long Index { get; set; }

    [JsonPropertyName("logprobs")]
    public object Logprobs { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public Message? Message { get; set; }
}

public class ContentFilterResults
{
    [JsonPropertyName("hate")]
    public Hate Hate { get; set; }

    [JsonPropertyName("self_harm")]
    public Hate SelfHarm { get; set; }

    [JsonPropertyName("sexual")]
    public Hate Sexual { get; set; }

    [JsonPropertyName("violence")]
    public Hate Violence { get; set; }
}

public class Hate
{
    [JsonPropertyName("filtered")]
    public bool Filtered { get; set; }

    [JsonPropertyName("severity")]
    public string Severity { get; set; } = string.Empty;
}

public class Message
{
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;
}

public class PromptFilterResult
{
    [JsonPropertyName("prompt_index")]
    public long PromptIndex { get; set; }

    [JsonPropertyName("content_filter_result")]
    public ContentFilterResult? ContentFilterResult { get; set; }
}

public class ContentFilterResult
{
    [JsonPropertyName("jailbreak")]
    public Jailbreak? Jailbreak { get; set; }

    [JsonPropertyName("custom_blocklists")]
    public CustomBlocklists? CustomBlocklists { get; set; }

    [JsonPropertyName("sexual")]
    public Hate? Sexual { get; set; }

    [JsonPropertyName("violence")]
    public Hate? Violence { get; set; }

    [JsonPropertyName("hate")]
    public Hate? Hate { get; set; }

    [JsonPropertyName("self_harm")]
    public Hate? SelfHarm { get; set; }
}

public class CustomBlocklists
{
    [JsonPropertyName("filtered")]
    public bool Filtered { get; set; }

    [JsonPropertyName("details")]
    public object[]? Details { get; set; }
}

public class Jailbreak
{
    [JsonPropertyName("filtered")]
    public bool Filtered { get; set; }

    [JsonPropertyName("detected")]
    public bool Detected { get; set; }
}

public class Usage
{
    [JsonPropertyName("completion_tokens")]
    public long CompletionTokens { get; set; }

    [JsonPropertyName("prompt_tokens")]
    public long PromptTokens { get; set; }

    [JsonPropertyName("total_tokens")]
    public long TotalTokens { get; set; }
}