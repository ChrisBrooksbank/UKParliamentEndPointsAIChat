using System.Text.Json.Serialization;

namespace UKParliamentEndPointsAIChat.Ui.OpenAi.Api.Functions
{
    public class FunctionParameters
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("properties")]
        public Dictionary<string, ParameterDetail> Properties { get; set; }

        [JsonPropertyName("required")]
        public List<string> Required { get; set; }
    }
}
