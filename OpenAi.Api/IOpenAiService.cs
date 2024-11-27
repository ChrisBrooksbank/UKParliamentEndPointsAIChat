using UKParliamentEndPointsAIChat.Ui.OpenAi.Api.Response;

namespace UKParliamentEndPointsAIChat.Ui.OpenAi.Api
{
    public interface IOpenAiService
    {
        Task<string> SendMessageAsync(string message, bool useApi = false);
    }
}
