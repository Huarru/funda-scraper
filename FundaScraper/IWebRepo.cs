using System;

namespace FundaScraper
{
    public interface IWebRepo
    {
        string GetHtml(Uri url);
    }
}