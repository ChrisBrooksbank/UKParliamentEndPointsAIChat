namespace UKParliamentEndPointsAIChat.Ui.OpenAi.Api.Functions
{
    public interface IFunctionBuilder
    {
        FunctionBuilder SetName(string name);
        FunctionBuilder SetDescription(string description);
        FunctionBuilder AddParam(string paramName, string type, string description, bool isRequired = false);
        FunctionBuilder SetApiUrl(string url);
    }
}
