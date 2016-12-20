using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeywordSearch.Models
{
    public class Search
    {
        public Search()
        {
            ResultList = new List<string>();
            TempResults = new List<Result>();
            Results = new List<Result>();
        }
        public string SearchWord { get; set; }
        public string Keyword { get; set; }
        public string url { get; set; }
        public string Result { get; set; }
        public List<string> ResultList { get; set; }
        public int Page { get; set; }
        
        public List<Result> TempResults { get; set; }
        public List<Result> Results { get; set; }
    }
}