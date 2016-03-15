using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

namespace FundaScraper
{
    public class FundaV1Parser : IParser
    {
        private readonly Uri _baseUrl;
        private CultureInfo nlCulture = CultureInfo.CreateSpecificCulture("nl-NL");
        public FundaV1Parser(string baseUrl)
        {
            _baseUrl = new Uri(baseUrl);
        }

        public FundaResult ParsePage(string data)
        {
            FundaResult result = new FundaResult();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(data);

            IEnumerable<HtmlNode> houseNodes = doc.DocumentNode.GetNodesForClass("div", "specs");

            foreach (HtmlNode house in houseNodes)
            {
                FundaObject fundaObject = new FundaObject();

                var streetHouseNumber = house.Descendants("h3").First().Descendants("a").First();
                fundaObject.StraatAdres = ParseAddress(streetHouseNumber.InnerText);
                var liItems = house.GetNodesForThatDoesNotHaveClass("li", "object-tagline").ToList();

                var surface = house.GetNodesForAttribute("span", "title", "Oppervlakte").FirstOrDefault();
                string surfaceValue = surface?.InnerText.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(surfaceValue))
                {
                    fundaObject.Surface = int.Parse(surfaceValue, NumberStyles.AllowThousands, nlCulture);
                }

                var pricesNode = house.GetNodesForClass("span", "price-wrapper").FirstOrDefault();
                if (pricesNode != null)
                {
                    var prices = pricesNode.InnerHtml.Split(new string[] {"<br>"}, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var s in prices)
                    {
                        HtmlDocument node = new HtmlDocument();
                        node.LoadHtml(s);
                        var price = node.DocumentNode.GetNodesForClass("span", "price").FirstOrDefault();
                        var priceExt = node.DocumentNode.GetNodesForClass("abbr", "price-ext").FirstOrDefault();

                        if (priceExt != null && priceExt.InnerText.Contains("Huurprijs") ||
                            (price != null && (price.InnerText.Contains("/mnd") || price.InnerText.Contains("/jr"))))
                        {
                            if (price != null)
                            {
                                fundaObject.Huurprijs = ParsePrice(price.InnerText);
                            }
                            if (priceExt != null)
                            {
                                fundaObject.HuurprijsSpec = WebUtility.HtmlDecode(priceExt.InnerText);
                            }
                            
                        }
                        else
                        {
                            if (price != null)
                            {
                                fundaObject.Koopprijs = ParsePrice(price.InnerText);
                            }
                            if (priceExt != null)
                            {
                                fundaObject.KoopprijsSpec = WebUtility.HtmlDecode(priceExt.InnerText);
                            }
                        }


                    }
                    
                }

                if (liItems.Count > 1)
                {
                    var address = liItems[0].InnerText;
                    fundaObject.PostCodePlaats = ParseZipCodeRow(address);
                }

                

                result.FundaObjects.Add(fundaObject);
            }

            var nextPage = doc.DocumentNode.GetNodesForClass("a", "paging next").ToList();
            if (nextPage.Any())
            {
                result.HasNextPage = true;
                string urlPath = nextPage[0].Attributes["href"].Value;
                result.NextPage = new Uri(_baseUrl, urlPath);
            }

            return result;
        }

        private string ParsePrice(string value)
        {
            string priceValue = WebUtility.HtmlDecode(value);
            if (priceValue.StartsWith("€"))
            {
                string valueWithoutEur = priceValue.Substring(1).Trim();
                return int.Parse(valueWithoutEur, NumberStyles.AllowThousands, nlCulture).ToString();
            }

            return value;
        }


        public PostCodePlaats ParseZipCodeRow(string row)
        {
            var parsedRow = row.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
            var postcodePlaats = new PostCodePlaats();
            postcodePlaats.Postcode = WebUtility.HtmlDecode (parsedRow[0]);
            postcodePlaats.Plaats = WebUtility.HtmlDecode (parsedRow[1]);
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

            int huisnr;
            if (adressParts.All(x => int.TryParse(x, out huisnr) == false))
            {
                straatAdres.Straatnaam = string.Join(" ", adressParts);
            }
            else
            {
                for (int i = adressParts.Length - 1; i >= 0; i--)
                {
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
            }
            return straatAdres;
        }
    }
}