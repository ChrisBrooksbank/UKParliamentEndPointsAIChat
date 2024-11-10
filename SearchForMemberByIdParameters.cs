using System.Text.Json.Serialization;

namespace UKParliamentEndPointsAIChat.Ui
{
    public class SearchForMemberByIdParameters
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}
