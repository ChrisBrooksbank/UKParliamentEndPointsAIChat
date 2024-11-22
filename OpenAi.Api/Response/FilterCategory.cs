using System.Text.Json.Serialization;

namespace UKParliamentEndPointsAIChat.Ui.OpenAi.Api.Response
{
    public class FilterCategory
    {
        [JsonPropertyName("filtered")]
        public bool Filtered { get; set; }

        [JsonPropertyName("severity")]
        public string Severity { get; set; }
    }
}
