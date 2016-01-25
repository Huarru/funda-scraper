using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace FundaScraper
{
    public static class HtmlNodeExtentions
    {
        public static IEnumerable<HtmlNode> GetNodesForClass(this HtmlNode document, string htmlElement, string cssClass)
        {
            return document.Descendants(htmlElement).Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains(cssClass));
        }

        public static IEnumerable<HtmlNode> GetNodesForThatDoesNotHaveClass(this HtmlNode document, string htmlElement, string cssClass)
        {
            return document.Descendants(htmlElement).Where(d => !d.Attributes.Contains("class") || !d.Attributes["class"].Value.Contains(cssClass));
        }
    }
}
