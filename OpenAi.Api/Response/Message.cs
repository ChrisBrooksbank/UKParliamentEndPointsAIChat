using System.Text.Json.Serialization;

namespace UKParliamentEndPointsAIChat.Ui.OpenAi.Api.Response
{
    public class Message
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }
    }
}
