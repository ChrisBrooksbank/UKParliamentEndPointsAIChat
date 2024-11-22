namespace UKParliamentEndPointsAIChat.Ui.OpenAi.Api.Functions
{
    public interface IFunctionRepository
    {
        Function[] GetAll();
        Function[] GetByName(string name);
    }
}
