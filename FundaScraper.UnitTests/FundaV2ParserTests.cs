using System;
using System.Linq;
using NUnit.Framework;

namespace FundaScraper.UnitTests
{
    [TestFixture]
    public class ParserTests
    {
        IParser parser = new FundaV2Parser("http://www.funda.nl");

        [Test]
        public void ShouldParseSinglePage()
        {
            
            var result = parser.ParsePage(Properties.Resources.FundaNewLayout);

            Assert.That(result.FundaObjects, Has.Count.EqualTo(15));
            var firstObject = result.FundaObjects.First();
            Assert.That(firstObject.StraatAdres.Straatnaam, Is.EqualTo("Tongerlose Hoefstraat"));
            Assert.That(firstObject.StraatAdres.Huisnummer, Is.EqualTo(48));
            Assert.That(firstObject.StraatAdres.HuisnummerToevoeging, Is.EqualTo(string.Empty));
            Assert.That(firstObject.PostCodePlaats.Postcode, Is.EqualTo("5046 NH"));
            Assert.That(firstObject.PostCodePlaats.Plaats, Is.EqualTo("Tilburg"));
            Assert.That(result.HasNextPage, Is.True);
            Assert.That(result.NextPage, Is.EqualTo(new Uri("http://www.funda.nl/koop/heel-nederland/1-dag/bestaande-bouw/p2/")));
        }

        [Test]
        public void ShouldParse()
        {
            FundaResult result = parser.ParsePage(Properties.Resources.SingleRowIncompletePostCodeNewLayout);
            Assert.That(result.FundaObjects, Has.Count.EqualTo(1));
            Assert.That(result.FundaObjects[0].PostCodePlaats.Plaats, Is.EqualTo("Lijnden"));
        }
    }
}

