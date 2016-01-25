using System;
using System.Collections.Generic;

namespace FundaScraper
{
    public class FundaResult
    {
        public FundaResult()
        {
            FundaObjects = new List<FundaObject>();
        }
        public IList<FundaObject> FundaObjects { get; set; }

        public bool HasNextPage { get; set; }

        public Uri NextPage { get; set; }
    }
}