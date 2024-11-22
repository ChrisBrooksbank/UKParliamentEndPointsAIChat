using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UKParliamentEndPointsAIChat.Ui.OpenAi.Api.Functions
{
    public class FunctionRepository: IFunctionRepository
    {
        private static Function[] _functions;

        public FunctionRepository()
        {
            _functions =  new[]
            {
                FunctionBuilder.Create()
                    .SetName("search_parliament_member")
                    .SetDescription("Search for UK Parliament members by name")
                    .AddParam("name", "string", "The full or partial name of the member", isRequired: true)
                    .SetApiUrl("https://members-api.parliament.uk/api/Members/Search?Name={name}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("get_member_by_id")
                    .SetDescription("Get UK Parliament member by id")
                    .AddParam("id", "integer", "The id of the member", isRequired: true)
                    .SetApiUrl("https://members-api.parliament.uk/api/Members/{id}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_treaties")
                    .SetDescription("Search UK parliament treaties")
                    .AddParam("SearchText", "string", "Search Text", isRequired: true)
                    .SetApiUrl("https://treaties-api.parliament.uk/api/Treaty?SearchText={searchText}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_roi")
                    .SetDescription("Search registered interests (ROI)")
                    .AddParam("MemberId", "integer", "Member Id", isRequired: true)
                    .SetApiUrl("https://interests-api.parliament.uk/api/v1/Interests/?MemberId={memberId}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_erskine_may")
                    .SetDescription("Search erskine may")
                    .AddParam("searchTerm", "string", "search term", isRequired: true)
                    .SetApiUrl("https://erskinemay-api.parliament.uk/api/Search/ParagraphSearchResults/{searchTerm}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_commons_divisions")
                    .SetDescription("Search commons divisions")
                    .AddParam("searchTerm", "string", "search term", isRequired:true)
                    .SetApiUrl("http://commonsvotes-api.parliament.uk/data/divisions.json/search?queryParameters.searchTerm={searchTerm}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_lords_divisions")
                    .SetDescription("Search lords divisions")
                    .AddParam("searchTerm", "string", "search term", isRequired: true)
                    .SetApiUrl("http://lordsvotes-api.parliament.uk/data/divisions/search?queryParameters.searchTerm={searchTerm}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_bills")
                    .SetDescription("Search bills")
                    .AddParam("searchTerm", "string", "search term", isRequired: true)
                    .SetApiUrl("https://bills-api.parliament.uk/api/v1/Bills?SearchTerm={searchTerm}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_committees")
                    .SetDescription("Search committees")
                    .AddParam("SearchTerm", "string", "search term", isRequired: true)
                    .SetApiUrl("https://committees-api.parliament.uk/api/Committees?SearchTerm={searchTerm}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_earlydaymotions")
                    .SetDescription("Search early day motions")
                    .AddParam("SearchTerm", "string", "search term", isRequired: true)
                    .SetApiUrl("https://oralquestionsandmotions-api.parliament.uk/EarlyDayMotions/list?parameters.searchTerm={searchTerm}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("happening_now_in_commons")
                    .SetDescription("What is happening now in the commons. Annunciator.")
                    .SetApiUrl("https://now-api.parliament.uk/api/Message/message/CommonsMain/current")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("happening_now_in_lords")
                    .SetDescription("What is happening now in the lords. Annunciator.")
                    .SetApiUrl("https://now-api.parliament.uk/api/Message/message/LordsMain/current")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_statutory_instruments")
                    .SetDescription("Search statutory instruments by name")
                    .AddParam("Name", "string", "name", isRequired: true)
                    .SetApiUrl("https://statutoryinstruments-api.parliament.uk/api/v2/StatutoryInstrument?Name={name}")
                    .Build()
            };
        }

        public Function[] GetAll()
        {
            return _functions;
        }

        public Function[] GetByName(string name)
        {
            return _functions.Where(f => f.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)).ToArray();
        }
    }
}
