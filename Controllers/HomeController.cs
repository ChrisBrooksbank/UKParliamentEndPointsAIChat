using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using Markdig;
using Newtonsoft.Json;
using UKParliamentEndPointsAIChat.Ui.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace UKParliamentEndPointsAIChat.Ui.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _coachAndFocusLLMApiKey;
        private readonly string _coachAndFocusLLMEndpoint;
        private List<object> _messages = new List<object>();
        private readonly HttpClient _llmHttpClient;
        private readonly HttpClient _apihttpClient;

        private const double AITemperature = 0.7;
        private const double AITop_p = 0.95;
        private const int AIMaxTokens = 800;
        private const bool AIUseStream = false;

        private const string SYSTEM_PROMPT =
            "You are a helpful, friendly, very smart AI assistant that helps people find information on UK parliament. " +
            "You will only find information relevant to UK parliament. " +
            "Any questions in other fields will yield a response saying you are only for UK Parliament data." +
            "https://www.parliament.uk/ is the primary source of data and whenever possible you should return a link to this site. " +
            "Only return a link if its a real link that returns a 200 when a GET request is issued. " +
            "Its vital you check any links are real links that return 200.  " + 
            "You should return any useful API links. You must look at webpage https://developer.parliament.uk/";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _coachAndFocusLLMApiKey = Environment.GetEnvironmentVariable("CoachAndFocusLLMApiKey");
            _coachAndFocusLLMEndpoint = Environment.GetEnvironmentVariable("CoachAndFocusLLMEndpoint");
            _llmHttpClient = new HttpClient();
            _llmHttpClient.DefaultRequestHeaders.Add("api-key", _coachAndFocusLLMApiKey);
            _apihttpClient = new HttpClient();
            _messages.Clear();
            _messages.Add(new {role = "system", content = new object[] {new{type = "text", text = SYSTEM_PROMPT}}});
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessageToAI(string userMessage)
        {
            var generateApiCalls = userMessage.StartsWith("API ", StringComparison.CurrentCultureIgnoreCase);
            
            var functions = new[]
            {
                new FunctionDefinition
                {
                    Name = "search_parliament_member",
                    Description = "Search for UK Parliament members by name",
                    Parameters = new
                    {
                        type = "object",
                        properties = new
                        {
                            name = new { type = "string", description = "The full or partial name of the member" },
                            skip = new { type = "integer", description = "Number of records to skip" },
                            take = new { type = "integer", description = "Number of records to take" }
                        },
                        required = new[] { "name" }
                    }
                },
                new FunctionDefinition
                {
                    Name = "get_member_by_id",
                    Description = "Get UK Parliament member by id",
                    Parameters = new
                    {
                        type = "object",
                        properties = new
                        {
                            id = new { type = "string", description = "The id of the member" }
                        },
                        required = new[] { "id" }
                    }
                }
            };

            var messages = HttpContext.Session.GetString("Messages");
            _messages = string.IsNullOrEmpty(messages) ? GetNewMessagesList() : JsonSerializer.Deserialize<List<object>>(messages);

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
                temperature = AITemperature,
                top_p = AITop_p,
                max_tokens = AIMaxTokens,
                stream = AIUseStream,
                functions = generateApiCalls ? functions : null,
                function_call = generateApiCalls ? "auto" : null
            };

            var response = await _llmHttpClient.PostAsync(_coachAndFocusLLMEndpoint, new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();


            dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);
            var choice = jsonResponse.choices[0];
            var finishReason = (string)choice.finish_reason;
            var finishedWithFunctionCall = finishReason == "function_call";

            if (finishedWithFunctionCall)
            {
                var functionCall = choice.message.function_call;
                var functionName = (string)functionCall.name;
                var arguments = JsonConvert.DeserializeObject<Dictionary<string, object>>((string)functionCall.arguments);

                if (functionName == "search_parliament_member")
                {
                    string name = arguments.ContainsKey("name") ? arguments["name"].ToString() : "";
                    int skip = arguments.ContainsKey("skip") ? Convert.ToInt32(arguments["skip"]) : 0;
                    int take = arguments.ContainsKey("take") ? Convert.ToInt32(arguments["take"]) : 20;

                    var apiUrl = $"https://members-api.parliament.uk/api/Members/Search?Name={Uri.EscapeDataString(name)}&skip={skip}&take={take}";

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

                if (functionName == "get_member_by_id")
                {
                    int id = arguments.ContainsKey("id") ? Convert.ToInt32(arguments["id"]) : 0;
                    var apiUrl = $"https://members-api.parliament.uk/api/Members/{id}";

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

            if (!finishedWithFunctionCall)
            {
                var responseData = JsonSerializer.Deserialize<GptResponse>(await response.Content.ReadAsStringAsync());
                var messageContent = responseData.Choices[0].Message.Content;
                _messages.Add(new
                {
                    role = "assistant",
                    content = new object[]
                    {
                        new
                        {
                            type = "text",
                            text = messageContent
                        }
                    }
                });
                HttpContext.Session.SetString("Messages", JsonSerializer.Serialize(_messages));

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

                ViewBag.ResponseMessage = htmlResponse;
            }

            return View("Index");
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private List<object> GetNewMessagesList()
        {
            return new List<object>
            {
                new
                {
                    role = "system",
                    content = new object[] {new {type = "text", text = SYSTEM_PROMPT}}
                }
            };
        }
    }
}
