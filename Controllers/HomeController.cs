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
        private readonly IFunctionDefinitionBuilder _functionDefinitionBuilder;
        private readonly string _coachAndFocusLLMApiKey;
        private readonly string _coachAndFocusLLMEndpoint;
        private List<object> _messages = new List<object>();
        private readonly HttpClient _llmHttpClient;
        private readonly HttpClient _apihttpClient;

        private const double AITemperature = 0.7;
        private const double AITop_p = 0.95;
        private const int AIMaxTokens = 4000;
        private const bool AIUseStream = false;

        private const string SYSTEM_PROMPT =
            "You are a helpful, friendly, very smart AI assistant that helps people find information on UK parliament. " +
            "You will only find information relevant to UK parliament. " +
            "Any questions in other fields will yield a response saying you are only for UK Parliament data." +
            "https://www.parliament.uk/ is the primary source of data and whenever possible you should return a link to this site. " +
            "Only return a link if its a real link that returns a 200 when a GET request is issued. " +
            "Its vital you check any links are real links that return 200.  " + 
            "You should return any useful API links. You must look at webpage https://developer.parliament.uk/";

        // saves LLM tokens
        private const bool ConfigContinuingConversation = false;
        private const bool ConfigUseFunctions = true;

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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> SendMessageToAI(string userMessage)
        {
            ViewBag.Request = userMessage;

            if (ConfigContinuingConversation)
            {
                var messages = HttpContext.Session.GetString("Messages");
                _messages = string.IsNullOrEmpty(messages) ? GetNewMessagesList() : JsonSerializer.Deserialize<List<object>>(messages);
            }
            else
            {
               _messages = GetNewMessagesList();
            }

            if (ConfigContinuingConversation && userMessage.ToLower() == "clear")
            {
                _messages.RemoveRange(1, _messages.Count - 1);
                HttpContext.Session.SetString("Messages", JsonSerializer.Serialize(_messages));
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

            if (ConfigContinuingConversation)
            {
                HttpContext.Session.SetString("Messages", JsonSerializer.Serialize(_messages));
            }

            var payload = GetPayLoad();
            
            var response = await _llmHttpClient.PostAsync(_coachAndFocusLLMEndpoint, new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));
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
                if (ConfigContinuingConversation)
                {
                    HttpContext.Session.SetString("Messages", JsonSerializer.Serialize(_messages));
                }

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

        private object GetPayLoad()
        {
            var functions = GetFunctionDefinitions();

            var payload = new
            {
                messages = _messages,
                temperature = AITemperature,
                top_p = AITop_p,
                max_tokens = AIMaxTokens,
                stream = AIUseStream,
                functions = ConfigUseFunctions ? functions : null,
                function_call = ConfigUseFunctions ? "auto" : null
            };
            return payload;
        }

        private async Task HandleFunctionResult(dynamic choice)
        {
            string functionName = GetFunctionName(choice, out Dictionary<string, object>? arguments);

            if (functionName == "search_parliament_member")
            {
                var name =  GetParameter<string>(arguments, "name");
                var skip = GetParameter<int>(arguments, "skip");
                var take = GetParameter<int>(arguments, "take", 20);;
                var apiUrl = $"https://members-api.parliament.uk/api/Members/Search?Name={Uri.EscapeDataString(name)}&skip={skip}&take={take}";
                await CallApi(apiUrl);
            }

            if (functionName == "get_member_by_id")
            {
                var id = GetParameter<int>(arguments, "Id");
                var apiUrl = $"https://members-api.parliament.uk/api/Members/{id}";
                await CallApi(apiUrl);
            }

            if (functionName == "search_treaties")
            {
                var searchText = GetParameter<string>(arguments, "SearchText");
                var apiUrl = $"https://treaties-api.parliament.uk/api/Treaty?SearchText={searchText}";
                await CallApi(apiUrl);
            }

            if (functionName == "search_roi")
            {
                var memberId = GetParameter<int>(arguments, "MemberId");
                var apiUrl = $"https://interests-api.parliament.uk/api/v1/Interests/?MemberId={memberId}";
                await CallApi(apiUrl);
            }

            if (functionName == "search_erskine_may")
            {
                var searchTerm = GetParameter<string>(arguments, "searchTerm");
                var apiUrl = $"https://erskinemay-api.parliament.uk/api/Search/ParagraphSearchResults/{searchTerm}";
                await CallApi(apiUrl);
            }

            if (functionName == "search_commons_divisions")
            {
                var searchTerm = GetParameter<string>(arguments, "searchTerm");
                var apiUrl = $"http://commonsvotes-api.parliament.uk/data/divisions.json/search?queryParameters.searchTerm={searchTerm}";
                await CallApi(apiUrl);
            }
            if (functionName == "search_lords_divisions")
            {
                var searchTerm = GetParameter<string>(arguments, "searchTerm");
                var apiUrl = $"http://lordsvotes-api.parliament.uk/data/divisions/search?queryParameters.searchTerm={searchTerm}";
                await CallApi(apiUrl);
            }

            if (functionName == "search_bills")
            {
                var searchTerm = GetParameter<string>(arguments, "searchTerm");
                var apiUrl = $"https://bills-api.parliament.uk/api/v1/Bills?SearchTerm={searchTerm}";
                await CallApi(apiUrl);
            }
            if (functionName == "search_committees")
            {
                var searchTerm = GetParameter<string>(arguments, "SearchTerm");
                var apiUrl = $"https://committees-api.parliament.uk/api/Committees?SearchTerm={searchTerm}";
                await CallApi(apiUrl);
            }
            if (functionName == "search_earlydaymotions")
            {
                var searchTerm = GetParameter<string>(arguments, "SearchTerm");
                var apiUrl = $"https://oralquestionsandmotions-api.parliament.uk/EarlyDayMotions/list?parameters.searchTerm={searchTerm}";
                await CallApi(apiUrl);
            }
            if (functionName == "happening_now_in_commons")
            {
                await CallApi("https://now-api.parliament.uk/api/Message/message/CommonsMain/current");
            }
            if (functionName == "happening_now_in_lords")
            {
                await CallApi("https://now-api.parliament.uk/api/Message/message/LordsMain/current");
            }
        }

        private static string GetFunctionName(dynamic choice, out Dictionary<string, object>? arguments)
        {
            var functionCall = choice.message.function_call;
            var functionName = (string)functionCall.name;
            arguments = JsonConvert.DeserializeObject<Dictionary<string, object>>((string)functionCall.arguments);
            return functionName;
        }

        private static FunctionDefinition[] GetFunctionDefinitions()
        {
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
                },
                new FunctionDefinition()
                {
                    Name = "search_treaties",
                    Description = "Search UK parliament treaties",
                    Parameters = new
                    {
                        type = "object",
                        properties = new
                        {
                            SearchText = new { type = "string", description = "Search Text" }
                        },
                        required = new[] { "SearchText" }
                    }
                },
                new FunctionDefinition()
                {
                    Name = "search_roi",
                    Description = "Search registered interests (ROI)",
                    Parameters = new
                    {
                        type = "object",
                        properties = new
                        {
                            MemberId = new { type = "integer", description = "Member Id" }
                        },
                        required = new[] { "MemberId" }
                    }
                },
                new FunctionDefinition()
                {
                    Name = "search_erskine_may",
                    Description = "Search erskine may",
                    Parameters = new
                    {
                        type = "object",
                        properties = new
                        {
                            searchTerm = new { type = "string", description = "search term" }
                        },
                        required = new[] { "searchTerm" }
                    }
                },
                new FunctionDefinition()
                {
                    Name = "search_commons_divisions",
                    Description = "Search commons divisions",
                    Parameters = new
                    {
                        type = "object",
                        properties = new
                        {
                            searchTerm = new { type = "string", description = "search term" },
                            startDate = new { type = "string", description = "start date" },
                            endDate = new { type = "string", description = "end date" }
                        }
                    }
                },
                new FunctionDefinition()
                {
                    Name = "search_lords_divisions",
                    Description = "Search commons divisions",
                    Parameters = new
                    {
                        type = "object",
                        properties = new
                        {
                            searchTerm = new { type = "string", description = "search term" }
                        },
                        required = new[] { "searchTerm" }
                    }
                },
                new FunctionDefinition()
                {
                    Name = "search_bills",
                    Description = "Search bills",
                    Parameters = new
                    {
                        type = "object",
                        properties = new
                        {
                            searchTerm = new { type = "string", description = "search term" }
                        },
                        required = new[] { "searchTerm" }
                    }
                },
                new FunctionDefinition()
                {
                    Name = "search_committees",
                    Description = "Search committees",
                    Parameters = new
                    {
                        type = "object",
                        properties = new
                        {
                            SearchTerm = new { type = "string", description = "search term" }
                        },
                        required = new[] { "SearchTerm" }
                    }
                },
                new FunctionDefinition()
                {
                    Name = "search_earlydaymotions",
                    Description = "Search early day motions",
                    Parameters = new
                    {
                        type = "object",
                        properties = new
                        {
                            SearchTerm = new { type = "string", description = "search term" }
                        },
                        required = new[] { "SearchTerm" }
                    }
                },
                new FunctionDefinition()
                {
                    Name = "happening_now_in_commons",
                    Description = "What is happening now in the commons. Annunciator.",
                },
                new FunctionDefinition()
                {
                    Name = "happening_now_in_lords",
                    Description = "What is happening now in the lords. Annunciator.",
                }
            };
            return functions;
        }
        
        private T GetParameter<T>(Dictionary<string, object> arguments, string key, T defaultValue = default(T))
        {
            if (arguments.ContainsKey(key))
            {
                return (T)Convert.ChangeType(arguments[key], typeof(T));
            }

            return defaultValue;
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
