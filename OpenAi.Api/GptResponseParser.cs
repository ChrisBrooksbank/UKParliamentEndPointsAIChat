using Markdig;
using Newtonsoft.Json;
using UKParliamentEndPointsAIChat.Ui.OpenAi.Api.Functions;
using UKParliamentEndPointsAIChat.Ui.OpenAi.Api.Response;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace UKParliamentEndPointsAIChat.Ui.OpenAi.Api
{
    public class GptResponseParser : IGptResponseParser
    {
        private IFunctionRepository _functionRepository;

        public GptResponseParser()
        {
            _functionRepository = new FunctionRepository();
        }

        public async Task<string> GetApiUrl(string gptResponse)
        {
            if (string.IsNullOrWhiteSpace(gptResponse))
            {
                return string.Empty;
            }

            dynamic jsonResponse = JsonConvert.DeserializeObject(gptResponse);
            if (jsonResponse == null)
            {
                return string.Empty;
            }

            var choice = jsonResponse.choices[0];
            var finishReason = (string)choice.finish_reason;
            return finishReason.Equals("function_call") ? await GetUrlFromFunctionResult(choice) : string.Empty;
        }

        public string GetHtml(string gptResponse)
        {
            if (string.IsNullOrWhiteSpace(gptResponse))
            {
                return string.Empty;
            }

            var responseData = JsonSerializer.Deserialize<GptResponse>(gptResponse);
            var messageContent = responseData.Choices[0].Message.Content;
            var htmlResponse = ParseAiMarkdownResponse(messageContent);
            return htmlResponse;
        }

        private async Task<string> GetUrlFromFunctionResult(dynamic choice)
        {
            string functionName = GetFunctionName(choice, out Dictionary<string, object>? arguments);
            var function = _functionRepository.GetByName(functionName).FirstOrDefault();
            if (function == null)
            {
                return string.Empty;
            }
            var url = GetUrl(function.ApiUrl, arguments);
            return url;
        }

        private static string GetFunctionName(dynamic choice, out Dictionary<string, object>? arguments)
        {
            var functionCall = choice.message.function_call;
            var functionName = (string)functionCall.name;
            arguments = JsonConvert.DeserializeObject<Dictionary<string, object>>((string)functionCall.arguments);
            return functionName;
        }

        private string GetUrl(string urlTemplate, Dictionary<string, object>? arguments)
        {
            if (string.IsNullOrEmpty(urlTemplate))
                throw new ArgumentException("URL template cannot be null or empty.", nameof(urlTemplate));

            if (arguments == null || arguments.Count == 0)
                return urlTemplate;

            string resolvedUrl = urlTemplate;

            foreach (var kvp in arguments)
            {
                string placeholder = $"{{{kvp.Key}}}";
                string value = kvp.Value != null ? Uri.EscapeDataString(kvp.Value.ToString()) : "";
                resolvedUrl = resolvedUrl.Replace(placeholder, value);
            }

            // Remove any query parameters with unresolved placeholders
            var uriBuilder = new UriBuilder(resolvedUrl);
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
            foreach (var key in query.AllKeys)
            {
                if (query[key]?.Contains("{") == true)
                {
                    query.Remove(key);
                }
            }
            uriBuilder.Query = query.ToString();

            return uriBuilder.ToString();
        }

        private static string ParseAiMarkdownResponse(string messageContent)
        {
            var htmlResponse = Markdown.ToHtml(messageContent);
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(htmlResponse);
            var links = htmlDoc.DocumentNode.SelectNodes("//a[@href]");
            if (links != null)
            {
                foreach (var link in links)
                {
                    link.SetAttributeValue("target", "_blank");
                }
            }
            htmlResponse = htmlDoc.DocumentNode.OuterHtml;
            return htmlResponse;
        }
    }
}
