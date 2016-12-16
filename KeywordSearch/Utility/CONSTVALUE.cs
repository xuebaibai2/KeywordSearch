using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeywordSearch.Utility
{
    public struct CONSTVALUE
    {
        public const string GOOGLE_URL = "https://www.google.com.au/search?q={0}&start={1}";

        public const string GOOGLE_REGEX = @"(<cite>)(.*?)(<\/cite>)";
    }
}