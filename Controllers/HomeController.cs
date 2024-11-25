using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UKParliamentEndPointsAIChat.Ui.Models;
using UKParliamentEndPointsAIChat.Ui.OpenAi.Api;
using UKParliamentEndPointsAIChat.Ui.OpenAi.Api.Functions;

namespace UKParliamentEndPointsAIChat.Ui.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOpenAiService _openAiService;
        private readonly HttpClient _apihttpClient;
        private readonly IGptResponseParser _gptResponseParser;
        private readonly IExampleRepository _exampleRepository;

        public HomeController()
        {
            _openAiService = new OpenAIService();
            _gptResponseParser = new GptResponseParser();
            _apihttpClient = new HttpClient();
            _exampleRepository = new ExampleRepository();
        }

        public IActionResult Index()
        {
            ViewBag.ExampleQueries = _exampleRepository.GetAll();
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
            ViewBag.ExampleQueries = _exampleRepository.GetAll();
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
