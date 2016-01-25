using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

namespace FundaScraper
{
    public class FundaV2Parser : IParser
    {
        private readonly Uri _baseUrl;

        public FundaV2Parser(string baseUrl)
        {
            _baseUrl = new Uri(baseUrl);
        }

        public FundaResult ParsePage(string data)
        {
            FundaResult result = new FundaResult();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(data);

            IEnumerable<HtmlNode> houseNodes = doc.DocumentNode.GetNodesForClass("div", "search-result-content-inner");

            foreach (HtmlNode house in houseNodes)
            {
                FundaObject fundaObject = new FundaObject();

                var streetHouseNumber = house.GetNodesForClass("h3", "search-result-title").First();
                fundaObject.StraatAdres = ParseAddress(streetHouseNumber.InnerText);
                fundaObject.PostCodePlaats = ParsePostcodePlaats(streetHouseNumber);

                result.FundaObjects.Add(fundaObject);
            }

            var nextPage = doc.DocumentNode.GetNodesForClass("li", "previous-next-page").ToList();
            foreach (HtmlNode node in nextPage)
            {
                var link = node.Descendants("a").First();
                if (link.InnerText.IndexOf("volgende", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    result.HasNextPage = true;
                    string urlPath = link.Attributes["href"].Value;
                    result.NextPage = new Uri(_baseUrl, urlPath);
                }
            }

            return result;
        }

        private PostCodePlaats ParsePostcodePlaats(HtmlNode innerText)
        {
            return ParseZipCodeRow(innerText.GetNodesForClass("small", "search-result-subtitle").First().InnerText);
        }


        public PostCodePlaats ParseZipCodeRow(string row)
        {
            var parsedRow = row.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();

            var postcodeplaats = parsedRow[0];

            string[] postcodeplaatsNodes = postcodeplaats.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);

            var postcodePlaats = new PostCodePlaats();
            if (postcodeplaatsNodes.Length == 2)
            {
                postcodePlaats.Postcode = WebUtility.HtmlDecode(postcodeplaatsNodes[0]);
                postcodePlaats.Plaats = WebUtility.HtmlDecode(postcodeplaatsNodes[1]);
            }
            else
            {
                postcodePlaats.Postcode = $"{WebUtility.HtmlDecode(postcodeplaatsNodes[0])} {WebUtility.HtmlDecode(postcodeplaatsNodes[1])}";
                postcodePlaats.Plaats = string.Empty;
                for (int i = 2; i < postcodeplaatsNodes.Length; i++)
                {
                    postcodePlaats.Plaats += $"{postcodeplaatsNodes[i]} ";
                }
            }

            if (postcodeplaatsNodes.Length == 3)
            {
                
                postcodePlaats.Plaats = WebUtility.HtmlDecode(postcodeplaatsNodes[2]);
            }
            return postcodePlaats;
        }

        public StraatAdres ParseAddress(string row)
        {
            StraatAdres straatAdres = new StraatAdres();
            straatAdres.Straatnaam = string.Empty;
            straatAdres.HuisnummerToevoeging = string.Empty;
            string adres = row.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => WebUtility.HtmlDecode (x.Trim())).First();

            string[] adressParts = adres.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            bool huisnummerFound = false;

            for (int i = adressParts.Length - 1; i >= 0; i--)
            {
                int huisnr;
                var part = adressParts[i];
                if (!huisnummerFound && int.TryParse(part, out huisnr))
                {
                    huisnummerFound = true;
                    straatAdres.Huisnummer = huisnr;
                    continue;
                }

                if (huisnummerFound)
                {
                    straatAdres.Straatnaam = part + " " + straatAdres.Straatnaam;
                }
                else
                {
                    straatAdres.HuisnummerToevoeging = part + " " + straatAdres.HuisnummerToevoeging;
                }
            }

            straatAdres.HuisnummerToevoeging = straatAdres.HuisnummerToevoeging.Trim();
            straatAdres.Straatnaam = straatAdres.Straatnaam.Trim();
            straatAdres.Adres = adres;

            return straatAdres;
        }
    }
}