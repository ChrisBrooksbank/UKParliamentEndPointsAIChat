﻿using static System.Runtime.InteropServices.JavaScript.JSType;

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
                    .AddParam("name", OpenApiParameterType.String, "The full or partial name of the member", isRequired: true)
                    .SetApiUrl("https://members-api.parliament.uk/api/Members/Search?Name={name}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("get_member_by_id")
                    .SetDescription("Get UK Parliament member by id")
                    .AddParam("id", OpenApiParameterType.Integer, "The id of the member", isRequired: true)
                    .SetApiUrl("https://members-api.parliament.uk/api/Members/{id}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_treaties")
                    .SetDescription("Search UK parliament treaties")
                    .AddParam("SearchText", OpenApiParameterType.String, "Search Text", isRequired: true)
                    .SetApiUrl("https://treaties-api.parliament.uk/api/Treaty?SearchText={searchText}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_roi")
                    .SetDescription("Search registered interests (ROI)")
                    .AddParam("MemberId", OpenApiParameterType.Integer, "Member Id", isRequired: true)
                    .SetApiUrl("https://interests-api.parliament.uk/api/v1/Interests/?MemberId={memberId}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_erskine_may")
                    .SetDescription("Search erskine may")
                    .AddParam("searchTerm", OpenApiParameterType.String, "search term", isRequired: true)
                    .SetApiUrl("https://erskinemay-api.parliament.uk/api/Search/ParagraphSearchResults/{searchTerm}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_commons_divisions")
                    .SetDescription("Search commons divisions")
                    .AddParam("searchTerm", OpenApiParameterType.String, "search term", isRequired:true)
                    .AddParam("memberId", OpenApiParameterType.Integer, "member id", isRequired:false)
                    .AddParam("queryParameters.startDate", OpenApiParameterType.String, "Start date in YYYY-MM-DD format", isRequired:false)
                    .AddParam("queryParameters.endDate", OpenApiParameterType.String, "End date in YYYY-MM-DD format", isRequired:false)
                    .AddParam("queryParameters.divisionNumber", OpenApiParameterType.Integer, "Division number", isRequired:false)
                    .SetApiUrl("http://commonsvotes-api.parliament.uk/data/divisions.json/search?queryParameters.searchTerm={searchTerm}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_lords_divisions")
                    .SetDescription("Search lords divisions")
                    .AddParam("searchTerm", OpenApiParameterType.String, "search term", isRequired: true)
                    .SetApiUrl("http://lordsvotes-api.parliament.uk/data/divisions/search?queryParameters.searchTerm={searchTerm}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_bills")
                    .SetDescription("Search bills")
                    .AddParam("searchTerm", OpenApiParameterType.String, "search term", isRequired: true)
                    .AddParam("MemberId", OpenApiParameterType.Integer, "member id", isRequired: false)
                    .SetApiUrl("https://bills-api.parliament.uk/api/v1/Bills?SearchTerm={searchTerm}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_committees")
                    .SetDescription("Search committees")
                    .AddParam("SearchTerm", OpenApiParameterType.String, "search term", isRequired: true)
                    .SetApiUrl("https://committees-api.parliament.uk/api/Committees?SearchTerm={searchTerm}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_earlydaymotions")
                    .SetDescription("Search early day motions")
                    .AddParam("SearchTerm", OpenApiParameterType.String, "search term", isRequired: true)
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
                    .AddParam("Name", OpenApiParameterType.String, "name", isRequired: true)
                    .SetApiUrl("https://statutoryinstruments-api.parliament.uk/api/v2/StatutoryInstrument?Name={name}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("edms_for_memberid")
                    .SetDescription("Get early day motions for member id")
                    .AddParam("memberid", OpenApiParameterType.Integer, "id of member", isRequired: true)
                    .SetApiUrl("https://members-api.parliament.uk/api/Members/{memberid}/Edms")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("parties_list_by_house")
                    .SetDescription("Get list of parties by house")
                    .AddParam("house", OpenApiParameterType.Integer, "house. 1 = commons, 2 = lords.", isRequired: true)
                    .SetApiUrl("https://members-api.parliament.uk/api/Parties/GetActive/{house}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("interests_categories")
                    .SetDescription("Get list of categories members can register interests in")
                    .SetApiUrl("https://interests-api.parliament.uk/api/v1/Categories")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("recently_updated_bills")
                    .SetDescription("Gets list of recently updated bills")
                    .AddParam("take", OpenApiParameterType.Integer, "max number of records to get", isRequired: false)
                    .SetApiUrl("https://bills-api.parliament.uk/api/v1/Bills?SortOrder=DateUpdatedDescending&skip=0&take={take}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("recently_tabled_edms")
                    .SetDescription("Gets list of recently tabled early day motions")
                    .AddParam("take", OpenApiParameterType.Integer, "max number of records to get", isRequired: false)
                    .SetApiUrl("https://oralquestionsandmotions-api.parliament.uk/EarlyDayMotions/list?parameters.orderBy=DateTabledDesc?skip=0&take={take}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("bill_types")
                    .SetDescription("Get list of bill types")
                    .SetApiUrl("https://bills-api.parliament.uk/api/v1/BillTypes")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("bill_stages")
                    .SetDescription("Get list of bill stages")
                    .SetApiUrl("https://bills-api.parliament.uk/api/v1/Stages")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("committee_meetings")
                    .SetDescription("Get list of committee meetings between two dates")
                    .AddParam("fromdate", OpenApiParameterType.String, "From date in format YYYY-MM-DD", isRequired: true)
                    .AddParam("todate", OpenApiParameterType.String, "To date in format YYYY-MM-DD", isRequired: true)
                    .SetApiUrl("https://committees-api.parliament.uk/api/Broadcast/Meetings?FromDate={fromdate}&ToDate={todate}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("get_departments")
                    .SetDescription("Get list of departments")
                    .SetApiUrl("https://members-api.parliament.uk/api/Reference/Departments")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("get_answering_bodies")
                    .SetDescription("Get list of answering bodies")
                    .SetApiUrl("https://members-api.parliament.uk/api/Reference/AnsweringBodies")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("get_committee_types")
                    .SetDescription("Get list of committee types")
                    .SetApiUrl("https://committees-api.parliament.uk/api/CommitteeType")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("get_contributions")
                    .SetDescription("Get contributions from a specified member")
                    .AddParam("memberid", OpenApiParameterType.Integer, "The id of the member", isRequired: true)
                    .SetApiUrl("https://members-api.parliament.uk/api/Members/{memberid}/ContributionSummary?page=1")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_hansard")
                    .SetDescription("Search hansard for contributions")
                    .AddParam("house", OpenApiParameterType.Integer, "house. 1 = commons, 2 = lords.", isRequired: true)
                    .AddParam("queryParameters.startDate", OpenApiParameterType.String, "Start date in YYYY-MM-DD format", isRequired:true)
                    .AddParam("queryParameters.endDate", OpenApiParameterType.String, "End date in YYYY-MM-DD format", isRequired:true)
                    .AddParam("searchTerm", OpenApiParameterType.String, "search term", isRequired: true)
                    .SetApiUrl("https://hansard-api.parliament.uk/search.json?queryParameters.house={house}&queryParameters.startDate={queryParameters.startDate}&queryParameters.endDate={queryParameters.startDate}&queryParameters.searchTerm={searchTerm}")
                    .Build(),
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
