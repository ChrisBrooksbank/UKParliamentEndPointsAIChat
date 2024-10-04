using System.Text.Json.Serialization;

namespace UKParliamentEndPointsAIChat.Ui
{
    public class GptResponse
    {
        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; }

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("prompt_filter_results")]
        public List<PromptFilterResult> PromptFilterResults { get; set; }

        [JsonPropertyName("system_fingerprint")]
        public string SystemFingerprint { get; set; }

        [JsonPropertyName("usage")]
        public Usage Usage { get; set; }
    }

    public class Choice
    {
        [JsonPropertyName("content_filter_results")]
        public ContentFilterResults ContentFilterResults { get; set; }

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; }

        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("logprobs")]
        public object Logprobs { get; set; }

        [JsonPropertyName("message")]
        public Message Message { get; set; }
    }

    public class ContentFilterResults
    {
        [JsonPropertyName("hate")]
        public FilterCategory Hate { get; set; }

        [JsonPropertyName("self_harm")]
        public FilterCategory SelfHarm { get; set; }

        [JsonPropertyName("sexual")]
        public FilterCategory Sexual { get; set; }

        [JsonPropertyName("violence")]
        public FilterCategory Violence { get; set; }
    }

    public class FilterCategory
    {
        [JsonPropertyName("filtered")]
        public bool Filtered { get; set; }

        [JsonPropertyName("severity")]
        public string Severity { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }
    }

    public class PromptFilterResult
    {
        [JsonPropertyName("prompt_index")]
        public int PromptIndex { get; set; }

        [JsonPropertyName("content_filter_results")]
        public ContentFilterResults ContentFilterResults { get; set; }
    }

    public class Usage
    {
        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }


}
