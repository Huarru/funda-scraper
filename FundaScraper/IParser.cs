namespace FundaScraper
{
    public interface IParser
    {
        PostCodePlaats ParseZipCodeRow(string row);
        StraatAdres ParseAddress(string row);
    }
}