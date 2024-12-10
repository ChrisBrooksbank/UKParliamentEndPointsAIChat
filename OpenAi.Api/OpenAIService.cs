using System.Net;
using OpenAI.Chat;
using System.Text;
using System.Text.Json;
using UKParliamentEndPointsAIChat.Ui.OpenAi.Api.Functions;
using Microsoft.Extensions.Options;
using UKParliamentEndPointsAIChat.Ui.Models;

namespace UKParliamentEndPointsAIChat.Ui.OpenAi.Api
{
    public class OpenAIService : IOpenAiService
    {
        private IFunctionRepository _functionRepository;
        private readonly Config _config;
        
        private readonly HttpClient _llmHttpClient;
        
        public OpenAIService(IOptions<Config> configOption)
        {
            _config = configOption.Value;

            _functionRepository = new FunctionRepository();
            _llmHttpClient = new HttpClient();

            SetAuthorizationHeader();
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
        
async Task<string> SendMessageAsyncAzure(string message)
        {
            var messages = new List<object>
            {
                new
                {
                    role = "system",
                    content = new object[] {new {type = "text", text = _config.SystemPrompt}}
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

            var functions = _functionRepository.GetAll(message);
            var payLoad = GetPayLoad(messages, functions);
            var payLoadJson = JsonSerializer.Serialize(payLoad);

            var content = new StringContent(payLoadJson, Encoding.UTF8, "application/json");

            try
            {
                var response = await _llmHttpClient.PostAsync(_config.CoachAndFocusLLMEndpoint, content);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == (HttpStatusCode)429)
            {
                return "Rate limit reached for requests to Open AI. Try again in sixty seconds.";
            }
        }

        private void SetAuthorizationHeader()
        {
            _llmHttpClient.DefaultRequestHeaders.Remove("Authorization");
            _llmHttpClient.DefaultRequestHeaders.Remove("api-key");

            if (_config.UseAzureHostedLlm)
            {
                _llmHttpClient.DefaultRequestHeaders.Add("api-key", _config.CoachAndFocusLLMApiKey);
            }
            else
            {
                _llmHttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.CoachAndFocusLLMApiKey2}");
            }
        }

        private object GetPayLoad(List<object> messages, Function[] functions)
        {
            var payload = new
            {
                messages = messages,
                temperature = _config.AITemperature,
                top_p = _config.AITop_p,
                max_tokens = _config.AIMaxTokens,
                stream = _config.AIUseStream,
                functions = functions,
                function_call = functions != null ? "auto" : null
            };
            return payload;
        }
    }
}
