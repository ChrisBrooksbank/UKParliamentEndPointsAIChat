using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UKParliamentEndPointsAIChat.Ui.Models;
using UKParliamentEndPointsAIChat.Ui.OpenAi.Api;

namespace UKParliamentEndPointsAIChat.Ui.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOpenAiService _openAiService;
        private readonly HttpClient _apihttpClient;
        private readonly IGptResponseParser _gptResponseParser;

        public HomeController()
        {
            _openAiService = new OpenAIService();
            _gptResponseParser = new GptResponseParser();
            _apihttpClient = new HttpClient();
        }

        public IActionResult Index()
        {
            var exampleQueries = new List<string>
            {
                "What is the function of UK Parliament",
                "Who is Boris Johnson",
                "What is happening now in Commons",
                "Statutory instruments on harbour",
                "What is happening now in Lords",
                "Who is member with id 1471",
                "Treaties on fish",
                "Treaties with Spain",
                "Present all the information in {CopyAndPastedApiResult}",
                "What are registered interests for member 172",
                "Search Erskine May for mace",
                "Search Commons Divisions for refugee",
                "Lords Divisions on refugee",
                "Bills on fish",
                "Committees on women",
                "Early day motions on fish",
                "Get early day motions for member 1471",
                "Get parties for house of commons",
                "Get parties for house of lords",
                "List categories for members interests",
                "List recently updated bills",
                "Get list of bill types",
                "Get list of bill stages"
            };

            ViewBag.ExampleQueries = exampleQueries;
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

            var url = await _gptResponseParser.GetApiUrl(responseContent);
            if (string.IsNullOrWhiteSpace(url))
            {
                ViewBag.ResponseMessage = _gptResponseParser.GetHtml(responseContent);
            }
            else
            {
                await CallApi(url);
            }

            return View("Index");
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
