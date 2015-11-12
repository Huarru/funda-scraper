using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;

namespace FundaScraper.UnitTests
{
    [TestFixture]
    public class FundaRepositoryTests
    {
        [Test]
        public void ShouldParseSinglePage()
        {
            IWebRepo webRepo = MockRepository.GenerateStub<IWebRepo>();
            webRepo.Expect(x => x.GetHtml(null))
                .IgnoreArguments()
                .Return(Properties.Resources.FundaResult);

            var scraper = new FundaRepository(webRepo, new Parser());

            FundaResult result = scraper.ParsePage(null);

            Assert.That(result.FundaObjects, Has.Count.EqualTo(15));
            var firstObject = result.FundaObjects.First();
            Assert.That(firstObject.StraatAdres.Straatnaam, Is.EqualTo("Egelantiersgracht"));
            Assert.That(firstObject.StraatAdres.Huisnummer, Is.EqualTo(33));
            Assert.That(firstObject.StraatAdres.HuisnummerToevoeging, Is.EqualTo("C"));
            Assert.That(firstObject.PostCodePlaats.Postcode, Is.EqualTo("1015 RC"));
            Assert.That(firstObject.PostCodePlaats.Plaats, Is.EqualTo("Amsterdam"));
            Assert.That(result.HasNextPage, Is.True);
            Assert.That(result.NextPage, Is.EqualTo(new Uri("http://www.funda.nl/koop/heel-nederland/1-dag/p2/")));
        }

        [Test, Explicit]
        public void ShouldParseAllPages()
        {
            var scraper = new FundaRepository(new WebRepo(), new Parser());
            IEnumerable<FundaObject> fundaObjects = scraper.GetAllHouses(new Uri("http://www.funda.nl/koop/heel-nederland/1-dag/bestaande-bouw/"));

            foreach (FundaObject fundaObject in fundaObjects)
            {
                Console.WriteLine("{0} {1} {2}", fundaObject.StraatAdres.Straatnaam, fundaObject.StraatAdres.Huisnummer, fundaObject.StraatAdres.HuisnummerToevoeging);
                Console.WriteLine("{0} {1}", fundaObject.PostCodePlaats.Postcode, fundaObject.PostCodePlaats.Plaats);
            }
        }

        [Test]
        public void ShouldParseSpecialCharactersCorrectly()
        {
            IWebRepo webRepo = MockRepository.GenerateStub<IWebRepo>();
            webRepo.Expect(x => x.GetHtml(null))
                .IgnoreArguments()
                .Return(Properties.Resources.SingleObjectWithSpecialCharacter);

            var scraper = new FundaRepository(webRepo, new Parser());
            FundaResult result = scraper.ParsePage(null);
            Assert.That(result.FundaObjects, Has.Count.EqualTo(1));
            Assert.That(result.FundaObjects[0].PostCodePlaats.Plaats, Is.EqualTo("'s-Graveland"));
        }
    }
}
