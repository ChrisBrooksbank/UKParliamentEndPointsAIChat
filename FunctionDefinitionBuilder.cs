namespace UKParliamentEndPointsAIChat.Ui
{
    public class FunctionDefinitionBuilder
    {
        private string _name;
        private string _description;
        private List<FunctionDefinitionParameter> _parameters = new List<FunctionDefinitionParameter>();
        
        public void Clear()
        {
            _name = null;
            _description = null;
            _parameters.Clear();
        }

        public FunctionDefinitionBuilder AddName(string name)
        {
            _name = name;
            return this;
        }

        public FunctionDefinitionBuilder AddDescription(string description)
        {
            _description = description;
            return this;
        }

        public FunctionDefinitionBuilder AddParameter(string type, string description)
        {
            _parameters.Add( new FunctionDefinitionParameter{ Type = type, Description = description});
            return this;
        }

        public FunctionDefinition Build()
        {
            var functionDefinition = new FunctionDefinition
            {
                Name = _name,
                Description = _description,
                Parameters = new List<FunctionDefinitionParameter>()
            };

            // TODO
            foreach (var parameter in _parameters)
            {
            }

            return functionDefinition;
        }
    }
}
