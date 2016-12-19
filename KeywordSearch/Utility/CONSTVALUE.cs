using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeywordSearch.Utility
{
    public struct CONSTVALUE
    {
        public const string GOOGLE_URL = "https://www.google.com.au/search?q={0}&start={1}";

        public const string GOOGLE_REGEX = @"(<cite class=""_Rm"">)(.*?)(<\/cite>)";

        public const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.79 Safari/537.36 Edge/14.14393";
    }
}