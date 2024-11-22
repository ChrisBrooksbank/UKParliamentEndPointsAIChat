using UKParliamentEndPointsAIChat.Ui.OpenAi.Api.Functions;

namespace UKParliamentEndPointsAIChat.Ui.OpenAi.Api.Response
{
    public class FunctionResponse
    {
        public Uri ApiUrl { get; set; }
        public Function Function { get; set; }
        public Dictionary<string, object> FunctionArguments;
    }
}
