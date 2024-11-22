using System.Text;
using System.Text.Json;
using UKParliamentEndPointsAIChat.Ui.OpenAi.Api.Functions;

namespace UKParliamentEndPointsAIChat.Ui.OpenAi.Api
{
    public class OpenAIService : IOpenAiService
    {
        private IFunctionRepository _functionRepository;

        private readonly string _coachAndFocusLLMApiKey;
        private readonly string _coachAndFocusLLMEndpoint;

        private const double AITemperature = 0.7;
        private const double AITop_p = 0.95;
        private const int AIMaxTokens = 4000;
        private const bool AIUseStream = false;

        private readonly HttpClient _llmHttpClient;

        private const string SYSTEM_PROMPT =
            "You are a helpful, friendly, very smart AI assistant that helps people find information on UK parliament. " +
            "You will only find information relevant to UK parliament. " +
            "Any questions in other fields will yield a response saying you are only for UK Parliament data." +
            "https://www.parliament.uk/ is the primary source of data and whenever possible you should return a link to this site. " +
            "Only return a link if its a real link that returns a 200 when a GET request is issued. " +
            "Its vital you check any links are real links that return 200.  " +
            "You should return any useful API links. You must look at webpage https://developer.parliament.uk/";

        public OpenAIService()
        {
            _coachAndFocusLLMApiKey = Environment.GetEnvironmentVariable("CoachAndFocusLLMApiKey");
            _coachAndFocusLLMEndpoint = Environment.GetEnvironmentVariable("CoachAndFocusLLMEndpoint");

            _functionRepository = new FunctionRepository();
            _llmHttpClient = new HttpClient();
            _llmHttpClient.DefaultRequestHeaders.Add("api-key", _coachAndFocusLLMApiKey);
        }

        public async Task<HttpResponseMessage> SendMessageAsync(string message)
        {
            var messages = new List<object>
            {
                new
                {
                    role = "system",
                    content = new object[] {new {type = "text", text = SYSTEM_PROMPT}}
                }
            };
            messages.Add(new
            {
                role = "user",
                content = new object[] {
                    new {
                        type = "text",
                        text = message
                    }
                }
            });
           
            var functions = _functionRepository.GetAll();
            var payLoad = GetPayLoad(messages, functions);
            var payLoadJson = JsonSerializer.Serialize(payLoad);

            var content = new StringContent(payLoadJson, Encoding.UTF8, "application/json");
            var response = await _llmHttpClient.PostAsync(_coachAndFocusLLMEndpoint, content);

            return response;
        }

        private object GetPayLoad(List<object> messages, Function[] functions)
        {
            var payload = new
            {
                messages = messages,
                temperature = AITemperature,
                top_p = AITop_p,
                max_tokens = AIMaxTokens,
                stream = AIUseStream,
                functions = functions,
                function_call = functions != null ? "auto" : null
            };
            return payload;
        }
    }
}
