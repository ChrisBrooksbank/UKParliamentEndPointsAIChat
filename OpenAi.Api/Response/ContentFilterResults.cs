using System.Text.Json.Serialization;

namespace UKParliamentEndPointsAIChat.Ui.OpenAi.Api.Response
{
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
}
