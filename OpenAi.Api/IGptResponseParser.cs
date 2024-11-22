namespace UKParliamentEndPointsAIChat.Ui.OpenAi.Api
{
    public interface IGptResponseParser
    {
        Task<string> GetApiUrl(string gptResponse);
        string GetHtml(string gptResponse);
    }
}
