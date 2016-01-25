namespace FundaScraper
{
    public interface IParser
    {
        FundaResult ParsePage(string data);
        PostCodePlaats ParseZipCodeRow(string row);
        StraatAdres ParseAddress(string row);
    }
}