using System;
using System.Linq;
using NUnit.Framework;

namespace FundaScraper.UnitTests
{
    [TestFixture]
    public class FundaV1ParserTests
    {
        [Test]
        public void ShouldParseSinglePage()
        {
            var parser = new FundaV1Parser("http://www.funda.nl");
            var result = parser.ParsePage(Properties.Resources.FundaResult);

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

        [Test]
        public void ShouldParseSpecialCharactersCorrectly()
        {
            var parser = new FundaV1Parser("http://www.funda.nl");

            FundaResult result = parser.ParsePage(Properties.Resources.SingleObjectWithSpecialCharacter);
            Assert.That(result.FundaObjects, Has.Count.EqualTo(1));
            Assert.That(result.FundaObjects[0].PostCodePlaats.Plaats, Is.EqualTo("'s-Graveland"));
        }
    }
}

