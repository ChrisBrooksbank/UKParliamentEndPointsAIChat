using OpenAI.Chat;
using System.Reflection;
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
            var useApi = false; // TODO Im overdue config here !

            var apiKey = useApi ? "CoachAndFocusLLMApiKey2" : "CoachAndFocusLLMApiKey";
            var endpoint = useApi ? "CoachAndFocusLLMEndpoint2" : "CoachAndFocusLLMEndpoint";

            _coachAndFocusLLMApiKey = Environment.GetEnvironmentVariable(apiKey);
            _coachAndFocusLLMEndpoint = Environment.GetEnvironmentVariable(endpoint);
            
            _functionRepository = new FunctionRepository();
            _llmHttpClient = new HttpClient();

            SetAuthorizationHeader(useApi);
        }

        public async Task<string> SendMessageAsync(string message, bool useApi = false)
        {
            var response = useApi ? await SendMessageAsyncApi(message) : await SendMessageAsyncAzure(message);
            return response;
        }

        // TODO rewrite this method to not use the OPENAI nuget package and then remove use of that package
        // Its use of functions demands c# functions to be defined in the code
        private async Task<string> SendMessageAsyncApi(string message)
        {
            // https://www.nuget.org/packages/OpenAI/
            ChatCompletionOptions options = new(){ };
            ChatClient client = new(model: "gpt-4o", apiKey: Environment.GetEnvironmentVariable("CoachAndFocusLLMApiKey2"));
            ChatCompletion completion = await client.CompleteChatAsync($"Say '{message}'");
            var response = completion.Content[0].Text;
            return response;
        }
        
        private async Task<string> SendMessageAsyncAzure(string message)
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
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }

        private void SetAuthorizationHeader(bool useApi)
        {
            _llmHttpClient.DefaultRequestHeaders.Remove("Authorization");
            _llmHttpClient.DefaultRequestHeaders.Remove("api-key");

            if (useApi)
            {
                _llmHttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_coachAndFocusLLMApiKey}");
            }
            else
            {
                _llmHttpClient.DefaultRequestHeaders.Add("api-key", _coachAndFocusLLMApiKey);
            }
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
