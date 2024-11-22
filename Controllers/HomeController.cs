using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Markdig;
using Newtonsoft.Json;
using UKParliamentEndPointsAIChat.Ui.Models;
using UKParliamentEndPointsAIChat.Ui.OpenAi.Api;
using UKParliamentEndPointsAIChat.Ui.OpenAi.Api.Functions;
using UKParliamentEndPointsAIChat.Ui.OpenAi.Api.Response;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace UKParliamentEndPointsAIChat.Ui.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOpenAiService _openAiService;
        private readonly HttpClient _apihttpClient;
        private IFunctionRepository _functionRepository;

        public HomeController()
        {
            _openAiService = new OpenAIService();
            _apihttpClient = new HttpClient();
            _functionRepository = new FunctionRepository();
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> SendMessageToAI(string userMessage)
        {
            ViewBag.Request = userMessage;
            
            var response = await _openAiService.SendMessageAsync(userMessage);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();

            dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);
            var choice = jsonResponse.choices[0];
            var finishReason = (string)choice.finish_reason;
            var finishedWithFunctionCall = finishReason == "function_call";

            if (finishedWithFunctionCall)
            {
                await HandleFunctionResult(choice);
            }

            if (!finishedWithFunctionCall)
            {
                var responseData = JsonSerializer.Deserialize<GptResponse>(await response.Content.ReadAsStringAsync());
                var messageContent = responseData.Choices[0].Message.Content;
                var htmlResponse = ParseAiMarkdownResponse(messageContent);
                ViewBag.ResponseMessage = htmlResponse;
            }

            return View("Index");
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
        
        private async Task HandleFunctionResult(dynamic choice)
        {
            string functionName = GetFunctionName(choice, out Dictionary<string, object>? arguments);
            var function = _functionRepository.GetByName(functionName).FirstOrDefault();
            if (function == null)
            {
                return;
            }
            var url = GetUrl(function.ApiUrl, arguments);
            await CallApi(url);
        }

        public static string GetUrl(string urlTemplate, Dictionary<string, object>? arguments)
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
            return resolvedUrl;
        }
        
        private static string GetFunctionName(dynamic choice, out Dictionary<string, object>? arguments)
        {
            var functionCall = choice.message.function_call;
            var functionName = (string)functionCall.name;
            arguments = JsonConvert.DeserializeObject<Dictionary<string, object>>((string)functionCall.arguments);
            return functionName;
        }
        
        private async Task CallApi(string apiUrl)
        {
            var urlMessage = $"<p>I created a API call for you <a href='{apiUrl}' target='_none'>{apiUrl}</a><p>";

            ViewBag.ResponseMessage = urlMessage;

            var apiResponse = await _apihttpClient.GetAsync(apiUrl);
            if (apiResponse.IsSuccessStatusCode)
            {
                var apiResponseContent = await apiResponse.Content.ReadAsStringAsync();
                ViewBag.ApiResponse = apiResponseContent;
            }
            else
            {
                ViewBag.ResponseMessage += "<p>API call failed</p>";
            }
        }
    }
}
