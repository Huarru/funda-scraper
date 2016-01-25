using System;
using System.Collections.Generic;
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
    }
}
