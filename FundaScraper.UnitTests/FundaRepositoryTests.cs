using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using NUnit.Framework;

namespace FundaScraper.UnitTests
{
    [TestFixture]
    public class FundaRepositoryTests
    {
        [Test, Explicit]
        public void ShouldParseAllPages()
        {
            var scraper = new FundaRepository(new WebRepo(), new FundaV2Parser("http://www.funda.nl"));
            IEnumerable<FundaObject> fundaObjects =
                scraper.GetAllHouses(new Uri("http://www.funda.nl/koop/heel-nederland/1-dag/bestaande-bouw/"));

            foreach (FundaObject fundaObject in fundaObjects)
            {
                Console.WriteLine("{0}", fundaObject.StraatAdres.Adres);
                Console.WriteLine("{0} {1}", fundaObject.PostCodePlaats.Postcode, fundaObject.PostCodePlaats.Plaats);
                Console.WriteLine();
            }
        }


        [Test, Explicit]
        public void ShouldParseFibZuidHollandAllPages()
        {
            var scraper = new FundaRepository(new WebRepo(), new FundaV1Parser("http://www.fundainbusiness.nl"));


            IList<UrlData> urls = new List<UrlData>()
            {
                new UrlData() { Type = "kantoor",Url="http://www.fundainbusiness.nl/kantoor/provincie-zuid-holland/"},
                new UrlData() { Type = "bedrijfshal",Url="http://www.fundainbusiness.nl/bedrijfshal/provincie-zuid-holland/",},
                new UrlData() { Type = "winkel",Url="http://www.fundainbusiness.nl/winkel/provincie-zuid-holland/"},
                new UrlData() { Type = "horeca",Url="http://www.fundainbusiness.nl/horeca/zuid-holland/"},
                new UrlData() { Type = "bouwgrond",Url="http://www.fundainbusiness.nl/bouwgrond/provincie-zuid-holland/"},
                new UrlData() { Type = "garagebox",Url="http://www.fundainbusiness.nl/overig/provincie-zuid-holland/garagebox/"},
                new UrlData() { Type = "praktijkruimte",Url="http://www.fundainbusiness.nl/overig/provincie-zuid-holland/praktijkruimte/"},
                new UrlData() { Type = "showroom",Url="http://www.fundainbusiness.nl/overig/provincie-zuid-holland/showroom/"},
            };

            var nowInNl = GetNlDateTime();
            using (TextWriter textWriter = File.CreateText("D:\\zh.csv"))
            {
                foreach (var urlData in urls)
                {
                    IEnumerable<FundaObject> fundaObjects = scraper.GetAllHouses(new Uri(urlData.Url));
                    var csv = new CsvWriter(textWriter);
                    csv.Configuration.Delimiter = ";";
                    foreach (FundaObject fundaObject in fundaObjects)
                    {
                        csv.WriteField(urlData.Type);
                        csv.WriteField(nowInNl);
                        csv.WriteField(fundaObject.StraatAdres.Straatnaam);
                        csv.WriteField(fundaObject.StraatAdres.Huisnummer);
                        csv.WriteField(fundaObject.StraatAdres.HuisnummerToevoeging);
                        csv.WriteField(fundaObject.PostCodePlaats.Postcode);
                        csv.WriteField(fundaObject.PostCodePlaats.Plaats);
                        csv.WriteField(fundaObject.Surface);
                        csv.WriteField(fundaObject.Huurprijs);
                        csv.WriteField(fundaObject.HuurprijsSpec);
                        csv.WriteField(fundaObject.Koopprijs);
                        csv.WriteField(fundaObject.KoopprijsSpec);
                        csv.NextRecord();
                    }
                }
            }


        }

        private DateTime GetNlDateTime()
        {
            string TimeZoneNetherlands = "W. Europe Standard Time";
            var nlTimeZoneId = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneNetherlands);
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, nlTimeZoneId);

        }


        private class UrlData
        {
            public string Url { get; set; }
            public string Type { get; set; }
        }
    }

}

