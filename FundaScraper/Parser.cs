using System;
using System.Linq;
using System.Net;

namespace FundaScraper
{
    public class Parser : IParser
    {
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

            return straatAdres;
        }
    }
}