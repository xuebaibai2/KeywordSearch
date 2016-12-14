using KeywordSearch.Models;
using KeywordSearch.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace KeywordSearch.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            Search search = new Search();
            search.Keyword = "online title search";
            search.url = "https://www.google.com.au";
            return View(search);
        }

        [HttpPost]
        public ActionResult Index(Search search)
        {
            string pre_url = PrepareURL(search);
            search.Page = 0;
            bool isEndPage = false;
            string regex = @"(<cite>)(.*?)(<\/cite>)";
            //List<string> resultList = new List<string>();
            List<Result> resultsList = new List<Result>();

            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                while (!isEndPage)
                {
                    search.url = string.Format(pre_url, search.Page.ToString());
                    search.Result = wc.DownloadString(search.url);
                    //resultList = GetResult(search, regex);
                    resultsList = GetResultList(search, regex);
                    if (resultsList.Count != 0 && search.Page<10)
                    {
                        //search.ResultList.AddRange(resultList);
                        search.Results.AddRange(resultsList);
                        search.Page += 10;
                    }
                    else
                    {
                        isEndPage = true;
                    }
                }
            }
            return View("Result", search);
        }
        [HttpGet]
        public ActionResult Result(Search search)
        {
            ViewBag.resultIndex = Enumerable.Range(0, search.Results.Count)
                .Where(i => search.Results[i].URL.Contains(search.Keyword))
                .ToList();
            
            return View(search);
        }

        private string PrepareURL(Search search)
        {
            if (search.url.Contains("google.com.au"))
            {
                string pre_url = CONSTVALUE.GOOGLE_URL;
                search.Keyword = search.Keyword.Replace(" ", "+");
                return string.Format(pre_url, search.Keyword, "{0}");
            }else
            {
                return "";
            }
        }

        private List<string> GetResult(Search search, string regex)
        {
            List<string> resultList = new List<string>();
            StringBuilder sb = new StringBuilder();
            Regex reg = new Regex(regex, RegexOptions.IgnoreCase);
            Match match = reg.Match(search.Result);
            while (match.Success)
            {
                string temp = match.Groups[2].Value.Replace("<b>", "").Replace("</b>", "");
                resultList.Add(temp);
                match = match.NextMatch();
            }
            return resultList;
        }
        private List<Result> GetResultList(Search search, string regex)
        {
            search.TempResults = new List<Result>();
            StringBuilder sb = new StringBuilder();
            Regex reg = new Regex(regex, RegexOptions.IgnoreCase);
            Match match = reg.Match(search.Result);
            while (match.Success)
            {
                string temp = match.Groups[2].Value.Replace("<b>", "").Replace("</b>", "");
                int currentPage = search.Page / 10 + 1;
                search.TempResults.Add(new Result() { Page = currentPage, URL = temp });
                match = match.NextMatch();
            }
            return search.TempResults;
        }
    }
}