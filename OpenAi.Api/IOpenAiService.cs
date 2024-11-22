using UKParliamentEndPointsAIChat.Ui.OpenAi.Api.Response;

namespace UKParliamentEndPointsAIChat.Ui.OpenAi.Api
{
    public interface IOpenAiService
    {
        Task<HttpResponseMessage> SendMessageAsync(string message);
    }
}
