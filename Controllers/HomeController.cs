using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using UKParliamentEndPointsAIChat.Ui.Models;

namespace UKParliamentEndPointsAIChat.Ui.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _coachAndFocusLLMApiKey;
        private readonly string _coachAndFocusLLMEndpoint;
        private readonly List<object> _messages = new List<object>();
        private readonly HttpClient _httpClient;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _coachAndFocusLLMApiKey = Environment.GetEnvironmentVariable("CoachAndFocusLLMApiKey");
            _coachAndFocusLLMEndpoint = Environment.GetEnvironmentVariable("CoachAndFocusLLMEndpoint");
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("api-key", _coachAndFocusLLMApiKey);
        

            var _llm_system_message = new
            {
                role = "system",
                content = new object[]
                {
                    new
                    {
                        type = "text",
                        text = "You are an AI assistant that helps people find information."
                    }
                }
            };
            _messages.Add(_llm_system_message);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessageToAI(string userMessage)
        {
            if (userMessage.ToLower() == "clear")
            {
                _messages.RemoveRange(1, _messages.Count - 1);
            }

            _messages.Add(new
            {
                role = "user",
                content = new object[] {
                    new {
                        type = "text",
                        text = userMessage
                    }
                }
            });

            var payload = new
            {
                messages = _messages,
                temperature = 0.7,
                top_p = 0.95,
                max_tokens = 800,
                stream = false
            };

            var response = await _httpClient.PostAsync(_coachAndFocusLLMEndpoint, new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var responseData = JsonSerializer.Deserialize<GptResponse>(await response.Content.ReadAsStringAsync());
                var messageContent = responseData.Choices[0].Message.Content;
                _messages.Add(new
                {
                    role = "assistant",
                    content = new object[] {
                        new {
                            type = "text",
                            text = messageContent
                        }
                    }
                });
                ViewBag.ResponseMessage = messageContent;
            }

            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
