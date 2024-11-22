using System.Text.Json.Serialization;

namespace UKParliamentEndPointsAIChat.Ui.OpenAi.Api.Response
{
    public class PromptFilterResult
    {
        [JsonPropertyName("prompt_index")]
        public int PromptIndex { get; set; }

        [JsonPropertyName("content_filter_results")]
        public ContentFilterResults ContentFilterResults { get; set; }
    }
}
