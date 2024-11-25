﻿namespace UKParliamentEndPointsAIChat.Ui.OpenAi.Api.Functions
{
    public class ExampleRepository : IExampleRepository
    {
        public List<string> GetAll()
        {
            return new List<string>
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
        }
    }
}
