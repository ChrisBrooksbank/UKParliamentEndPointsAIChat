using System.Text.Json.Serialization;

namespace UKParliamentEndPointsAIChat.Ui
{
    public class SearchForMemberByNameParameters
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("skip")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Skip { get; set; } = 0;

        [JsonPropertyName("take")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Take { get; set; } = 20;
    }
}
