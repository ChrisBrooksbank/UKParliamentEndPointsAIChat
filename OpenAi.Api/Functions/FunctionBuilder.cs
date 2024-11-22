using UKParliamentEndPointsAIChat.Ui.OpenAi.Api.Functions;

public class FunctionBuilder: IFunctionBuilder
{
    private readonly Function _function;
    private readonly FunctionParameters _parameters;
    private readonly Dictionary<string, ParameterDetail> _properties;
    private readonly List<string> _requiredParameters;

    private bool _hasParameters;

    public FunctionBuilder()
    {
        _function = new Function();
        _parameters = new FunctionParameters
        {
            Type = "object",
            Properties = new Dictionary<string, ParameterDetail>()
        };
        _properties = _parameters.Properties;
        _requiredParameters = new List<string>();
        _hasParameters = false;
    }

    public static FunctionBuilder Create()
    {
        return new FunctionBuilder();
    }

    public FunctionBuilder SetName(string name)
    {
        _function.Name = name;
        return this;
    }

    public FunctionBuilder SetDescription(string description)
    {
        _function.Description = description;
        return this;
    }

    public FunctionBuilder AddParam(string paramName, string type, string description, bool isRequired = false)
    {
        _hasParameters = true;
        _properties[paramName] = new ParameterDetail
        {
            Type = type,
            Description = description
        };

        if (isRequired)
        {
            _requiredParameters.Add(paramName);
        }

        return this;
    }

    public Function Build()
    {
        if (_hasParameters)
        {
            if (_requiredParameters.Count > 0)
            {
                _parameters.Required = _requiredParameters;
            }

            _function.Parameters = _parameters;
        }

        // If there are no parameters, we omit the Parameters property (set to null)
        return _function;
    }
}