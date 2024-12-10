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
                    .SetHint("member")
                    .AddParam("name", OpenApiParameterType.String, "The full or partial name of the member", isRequired: true)
                    .SetApiUrl("https://members-api.parliament.uk/api/Members/Search?Name={name}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("get_member_by_id")
                    .SetDescription("Get UK Parliament member by id")
                    .SetHint("member")
                    .AddParam("id", OpenApiParameterType.Integer, "The id of the member", isRequired: true)
                    .SetApiUrl("https://members-api.parliament.uk/api/Members/{id}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_treaties")
                    .SetDescription("Search UK parliament treaties")
                    .SetHint("treaty, treaties")
                    .AddParam("SearchText", OpenApiParameterType.String, "Search Text", isRequired: true)
                    .SetApiUrl("https://treaties-api.parliament.uk/api/Treaty?SearchText={searchText}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_roi")
                    .SetDescription("Search registered interests (ROI)")
                    .SetHint("roi, interest, registered interest")
                    .AddParam("MemberId", OpenApiParameterType.Integer, "Member Id", isRequired: true)
                    .SetApiUrl("https://interests-api.parliament.uk/api/v1/Interests/?MemberId={memberId}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_erskine_may")
                    .SetDescription("Search erskine may")
                    .SetHint("erskine may")
                    .AddParam("searchTerm", OpenApiParameterType.String, "search term", isRequired: true)
                    .SetApiUrl("https://erskinemay-api.parliament.uk/api/Search/ParagraphSearchResults/{searchTerm}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_commons_divisions")
                    .SetDescription("Search commons divisions")
                    .SetHint("commons vote, commons division")
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
                    .SetHint("lords vote, lords division")
                    .AddParam("searchTerm", OpenApiParameterType.String, "search term", isRequired: true)
                    .SetApiUrl("http://lordsvotes-api.parliament.uk/data/divisions/search?queryParameters.searchTerm={searchTerm}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_bills")
                    .SetDescription("Search bills")
                    .SetHint("bill, bills")
                    .AddParam("searchTerm", OpenApiParameterType.String, "search term", isRequired: true)
                    .AddParam("MemberId", OpenApiParameterType.Integer, "member id", isRequired: false)
                    .SetApiUrl("https://bills-api.parliament.uk/api/v1/Bills?SearchTerm={searchTerm}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_committees")
                    .SetDescription("Search committees")
                    .SetHint("committee, committees")
                    .AddParam("SearchTerm", OpenApiParameterType.String, "search term", isRequired: true)
                    .SetApiUrl("https://committees-api.parliament.uk/api/Committees?SearchTerm={searchTerm}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_earlydaymotions")
                    .SetDescription("Search early day motions")
                    .SetHint("edm, early day motion")
                    .AddParam("SearchTerm", OpenApiParameterType.String, "search term", isRequired: true)
                    .SetApiUrl("https://oralquestionsandmotions-api.parliament.uk/EarlyDayMotions/list?parameters.searchTerm={searchTerm}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("happening_now_in_commons")
                    .SetDescription("What is happening now in the commons. Annunciator.")
                    .SetHint("commons, house of commons, now")
                    .SetApiUrl("https://now-api.parliament.uk/api/Message/message/CommonsMain/current")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("happening_now_in_lords")
                    .SetDescription("What is happening now in the lords. Annunciator.")
                    .SetHint("lords, house of lords, now")
                    .SetApiUrl("https://now-api.parliament.uk/api/Message/message/LordsMain/current")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_statutory_instruments")
                    .SetDescription("Search statutory instruments by name")
                    .SetHint("si, statutory instrument")
                    .AddParam("Name", OpenApiParameterType.String, "name", isRequired: true)
                    .SetApiUrl("https://statutoryinstruments-api.parliament.uk/api/v2/StatutoryInstrument?Name={name}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("edms_for_memberid")
                    .SetDescription("Get early day motions for member id")
                    .SetHint("edm, early day motion")
                    .AddParam("memberid", OpenApiParameterType.Integer, "id of member", isRequired: true)
                    .SetApiUrl("https://members-api.parliament.uk/api/Members/{memberid}/Edms")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("parties_list_by_house")
                    .SetDescription("Get list of parties by house")
                    .SetHint("parties, party")
                    .AddParam("house", OpenApiParameterType.Integer, "house. 1 = commons, 2 = lords.", isRequired: true)
                    .SetApiUrl("https://members-api.parliament.uk/api/Parties/GetActive/{house}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("interests_categories")
                    .SetDescription("Get list of categories members can register interests in")
                    .SetHint("interest, registered interest, categories")
                    .SetApiUrl("https://interests-api.parliament.uk/api/v1/Categories")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("recently_updated_bills")
                    .SetDescription("Gets list of recently updated bills")
                    .SetHint("bill, bills, recent, updated")
                    .AddParam("take", OpenApiParameterType.Integer, "max number of records to get", isRequired: false)
                    .SetApiUrl("https://bills-api.parliament.uk/api/v1/Bills?SortOrder=DateUpdatedDescending&skip=0&take={take}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("recently_tabled_edms")
                    .SetDescription("Gets list of recently tabled early day motions")
                    .SetHint("edm, early day motion, recent, tabled")
                    .AddParam("take", OpenApiParameterType.Integer, "max number of records to get", isRequired: false)
                    .SetApiUrl("https://oralquestionsandmotions-api.parliament.uk/EarlyDayMotions/list?parameters.orderBy=DateTabledDesc?skip=0&take={take}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("bill_types")
                    .SetDescription("Get list of bill types")
                    .SetHint("bill, bills, types")
                    .SetApiUrl("https://bills-api.parliament.uk/api/v1/BillTypes")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("bill_stages")
                    .SetDescription("Get list of bill stages")
                    .SetHint("bill, bills, stages")
                    .SetApiUrl("https://bills-api.parliament.uk/api/v1/Stages")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("committee_meetings")
                    .SetDescription("Get list of committee meetings between two dates")
                    .SetHint("committee, committees, meetings")
                    .AddParam("fromdate", OpenApiParameterType.String, "From date in format YYYY-MM-DD", isRequired: true)
                    .AddParam("todate", OpenApiParameterType.String, "To date in format YYYY-MM-DD", isRequired: true)
                    .SetApiUrl("https://committees-api.parliament.uk/api/Broadcast/Meetings?FromDate={fromdate}&ToDate={todate}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("get_departments")
                    .SetDescription("Get list of departments")
                    .SetHint("department, departments")
                    .SetApiUrl("https://members-api.parliament.uk/api/Reference/Departments")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("get_answering_bodies")
                    .SetDescription("Get list of answering bodies")
                    .SetHint("answering body, answering bodies")
                    .SetApiUrl("https://members-api.parliament.uk/api/Reference/AnsweringBodies")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("get_committee_types")
                    .SetDescription("Get list of committee types")
                    .SetHint("committee, committees, types")
                    .SetApiUrl("https://committees-api.parliament.uk/api/CommitteeType")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("get_contributions")
                    .SetDescription("Get contributions from a specified member")
                    .SetHint("contribution, contributions, member")
                    .AddParam("memberid", OpenApiParameterType.Integer, "The id of the member", isRequired: true)
                    .SetApiUrl("https://members-api.parliament.uk/api/Members/{memberid}/ContributionSummary?page=1")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("search_hansard")
                    .SetDescription("Search hansard for contributions")
                    .SetHint("contribution, contributions, hansard")
                    .AddParam("house", OpenApiParameterType.Integer, "house. 1 = commons, 2 = lords.", isRequired: true)
                    .AddParam("queryParameters.startDate", OpenApiParameterType.String, "Start date in YYYY-MM-DD format", isRequired:true)
                    .AddParam("queryParameters.endDate", OpenApiParameterType.String, "End date in YYYY-MM-DD format", isRequired:true)
                    .AddParam("searchTerm", OpenApiParameterType.String, "search term", isRequired: true)
                    .SetApiUrl("https://hansard-api.parliament.uk/search.json?queryParameters.house={house}&queryParameters.startDate={queryParameters.startDate}&queryParameters.endDate={queryParameters.startDate}&queryParameters.searchTerm={searchTerm}")
                    .Build(),

                  
                FunctionBuilder.Create()
                    .SetName("search_oral_question_times")
                    .SetDescription("Get oral question times")
                    .SetHint("oqt, oral question, question, time, times")
                    .AddParam("parameters.answeringDateStart", OpenApiParameterType.String, "Start date in YYYY-MM-DD format", isRequired:true)
                    .AddParam("parameters.answeringDateEnd", OpenApiParameterType.String, "End date in YYYY-MM-DD format", isRequired:true)                  
                    .SetApiUrl("https://oralquestionsandmotions-api.parliament.uk/oralquestiontimes/list?parameters.answeringDateStart={parameters.answeringDateStart}&parameters.answeringDateEnd={parameters.answeringDateEnd}")
                    .Build(),

                 
                FunctionBuilder.Create()
                    .SetName("get_registers_of_interests")
                    .SetDescription("get published registers of interests")                   
                    .SetHint("register, interest, interests")
                    .SetApiUrl("https://interests-api.parliament.uk/api/v1/Registers")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("get_constituencies")
                    .SetDescription("get list of constituencies")                 
                    .SetHint("constituency, constituencies")
                    .AddParam("skip", OpenApiParameterType.Integer, "number of records to skip", isRequired: false)  
                    .AddParam("take", OpenApiParameterType.Integer, "max number of records to get", isRequired: false)  
                    .SetApiUrl("https://members-api.parliament.uk/api/Location/Constituency/Search?&skip={skip}&take={take}")
                    .Build(),

                FunctionBuilder.Create()
                    .SetName("get_election_results_for_constituency")
                    .SetDescription("get election results for a constituency")                 
                    .SetHint("election, results, constituency")
                    .AddParam("constituencyid", OpenApiParameterType.Integer, "id of constituencyid to fetch election results for", isRequired: true)                   
                    .SetApiUrl("https://members-api.parliament.uk/api/Location/Constituency/{constituencyid}/ElectionResults")
                    .Build(),

                FunctionBuilder.Create()
                        .SetName("get_commons_voting_record_for_member")
                        .SetDescription("get commons voting i.e. division results for member")                 
                        .SetHint("vote, voting, divisions, member")
                        .AddParam("queryParameters.memberId", OpenApiParameterType.Integer, "id of member to fetching voting i.e. divisions record for", isRequired: true)                   
                        .SetApiUrl("https://commonsvotes-api.parliament.uk/data/divisions.json/membervoting?queryParameters.memberId={queryParameters.memberId}")
                        .Build(),
                    
                 FunctionBuilder.Create()
                        .SetName("get_lords_voting_record_for_member")
                        .SetDescription("get lords voting i.e. division results for member")                 
                        .SetHint("vote, voting, divisions, member")
                        .AddParam("MemberId", OpenApiParameterType.Integer, "id of member to fetching voting i.e. divisions record for", isRequired: true)                   
                        .SetApiUrl("https://lordsvotes-api.parliament.uk/data/Divisions/membervoting?MemberId={MemberId}")
                        .Build(),

                 FunctionBuilder.Create()
                        .SetName("get_lords_interests_staff")
                        .SetDescription("get lords interests staff")                 
                        .SetHint("interests, staff")
                        .AddParam("searchterm", OpenApiParameterType.String, "search term", isRequired: false)                   
                        .SetApiUrl("https://members-api.parliament.uk/api/LordsInterests/Staff?searchTerm=richard")
                        .Build(),

                 FunctionBuilder.Create()
                        .SetName("search_acts_of_parliament")
                        .SetDescription("search acts of parliament")                 
                        .SetHint("acts, act")
                        .AddParam("name", OpenApiParameterType.String, "Name", isRequired: true)                   
                        .SetApiUrl("https://statutoryinstruments-api.parliament.uk/api/v2/ActOfParliament?Name={name}")
                        .Build(),
            };
        }

        // TODO use hint to restrict queries
        public Function[] GetAll(string forQuery = null)
        {
            return _functions;
        }

        public Function[] GetByName(string name)
        {
            return _functions.Where(f => f.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)).ToArray();
        }
    }
}
