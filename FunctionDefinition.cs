using System.Text.Json.Serialization;

namespace UKParliamentEndPointsAIChat.Ui
{
    public class FunctionDefinition
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("parameters")]
        public object Parameters { get; set; }
    }
}
