namespace UKParliamentEndPointsAIChat.Ui
{
    public interface IFunctionDefinitionBuilder
    {
        void Clear();
        FunctionDefinitionBuilder AddName(string name);
        FunctionDefinitionBuilder AddDescription(string description);
        FunctionDefinitionBuilder AddParameter(string type, string description);
        FunctionDefinition Build();
    }
}
