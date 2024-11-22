using System.Text.Json.Serialization;

namespace UKParliamentEndPointsAIChat.Ui.OpenAi.Api.Functions
{
    public class Function
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("parameters")]
        public FunctionParameters Parameters { get; set; }

        [JsonIgnore]
        public string ApiUrl { get; set; }
    }
}
