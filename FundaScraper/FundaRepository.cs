using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace FundaScraper
{
    public class FundaRepository
    {
        private readonly IWebRepo _webRepo;
        private readonly IParser _parser;

        public FundaRepository(IWebRepo webRepo, IParser parser)
        {
            _webRepo = webRepo;
            _parser = parser;
        }

        public IEnumerable<FundaObject> GetAllHouses(Uri startUrl)
        {
            bool loop = true;
            Uri urlToParse = startUrl;

            while (loop)
            {
                string data = _webRepo.GetHtml(urlToParse);
                FundaResult result = _parser.ParsePage(data);
                foreach (var fundaObject in result.FundaObjects)
                {
                    yield return fundaObject;
                }
                loop = result.HasNextPage;
                urlToParse = result.NextPage;
            }
        }
    }
}
