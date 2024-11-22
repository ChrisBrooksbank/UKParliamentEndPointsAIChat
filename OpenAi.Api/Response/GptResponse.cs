using System.Text.Json.Serialization;

namespace UKParliamentEndPointsAIChat.Ui.OpenAi.Api.Response
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
}
