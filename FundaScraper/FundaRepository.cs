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

        private readonly Uri _fundaBaseUrl = new Uri("http://www.funda.nl");

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
                FundaResult result = ParsePage(urlToParse);
                foreach (var fundaObject in result.FundaObjects)
                {
                    yield return fundaObject;
                }
                loop = result.HasNextPage;
                urlToParse = result.NextPage;
            }
        }

        public FundaResult ParsePage(Uri url)
        {
            FundaResult result = new FundaResult();

            HtmlDocument doc = new HtmlDocument();
            string data = _webRepo.GetHtml(url);
            doc.LoadHtml(data);

            IEnumerable<HtmlNode> houseNodes = GetNodesForClass(doc.DocumentNode, "div", "specs");

            foreach (HtmlNode house in houseNodes)
            {
                FundaObject fundaObject = new FundaObject();

                var streetHouseNumber = house.Descendants("h3").First().Descendants("a").First();
                fundaObject.StraatAdres = _parser.ParseAddress(streetHouseNumber.InnerText);
                var liItems = GetNodesForThatDoesNotHaveClass(house, "li", "object-tagline").ToList();

                if (liItems.Count > 1)
                {
                    var address = liItems[0].InnerText;
                    fundaObject.PostCodePlaats = _parser.ParseZipCodeRow(address);
                }

                result.FundaObjects.Add(fundaObject);
            }

            var nextPage = GetNodesForClass(doc.DocumentNode, "a", "paging next").ToList();
            if (nextPage.Any())
            {
                result.HasNextPage = true;
                string urlPath = nextPage[0].Attributes["href"].Value;
                result.NextPage = new Uri(_fundaBaseUrl, urlPath);
            }

            return result;
        }

        public IEnumerable<HtmlNode> GetNodesForClass(HtmlNode document, string htmlElement, string cssClass)
        {
            return document.Descendants(htmlElement).Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains(cssClass));
        }

        public IEnumerable<HtmlNode> GetNodesForThatDoesNotHaveClass(HtmlNode document, string htmlElement, string cssClass)
        {
            return document.Descendants(htmlElement).Where(d => !d.Attributes.Contains("class") || !d.Attributes["class"].Value.Contains(cssClass));
        }
    }
}
