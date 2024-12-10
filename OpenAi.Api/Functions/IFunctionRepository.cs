namespace UKParliamentEndPointsAIChat.Ui.OpenAi.Api.Functions
{
    public interface IFunctionRepository
    {
        Function[] GetAll(string forQuery = null);
        Function[] GetByName(string name);
    }
}
