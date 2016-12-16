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
using System.Threading.Tasks;

namespace KeywordSearch.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            Search search = new Search();
            search.SearchWord = "online title search";
            search.url = "https://www.google.com.au";
            return View(search);
        }

        //[HttpPost]
        //public ActionResult Index(Search search)
        //{
        //    string pre_url = PrepareURL(search);
        //    search.Page = 0;
        //    bool isEndPage = false;
        //    string regex = @"(<cite>)(.*?)(<\/cite>)"; //Will Change based on User-Agent Header

        //    using (WebClient wc = new WebClient())
        //    {
        //        wc.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
        //        wc.Headers.Add("Content-Type", "text/plain;charset=UTF-8");
        //        //wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36");
        //        while (!isEndPage)
        //        {
        //            search.url = string.Format(pre_url, search.Page.ToString());
        //            search.Result = wc.DownloadString(search.url);
        //            GetResultList(search, regex);
        //            if (search.TempResults.Count != 0 && search.Page<=10)
        //            {
        //                search.Results.AddRange(search.TempResults);
        //                search.Page += 10;
        //            }
        //            else
        //            {
        //                isEndPage = true;
        //                search.Result = "";
        //                TempData["ResultList"] = search.Results;
        //            }
        //        }
        //    }
            
        //    return RedirectToAction("Result", search);
        //}

        [HttpPost]
        public ActionResult Index(Search search)
        {
            string pre_url = PrepareURL(search);
            search.SearchAmount = 20;
            search.Page = 0;
            bool isEndPage = false;
            string regex = CONSTVALUE.GOOGLE_REGEX; //Will Change based on User-Agent Header
            int searchPage = GetPageLoopBySearchAmount(search.SearchAmount); //Page start from 0

            for (int i = 0; i < searchPage; i++)
            {
                int urlPage = GetSearchPageByPageLoop(i);
                search.url = string.Format(pre_url, urlPage);
                search.Results.AddRange(GetResultlistAsync(GetHTMLComtent(search.url).Result, urlPage, regex).Result);
            }


            TempData["ResultList"] = search.Results;

            return RedirectToAction("Result", search);
        }

        [HttpGet]
        public ActionResult Result(Search search)
        {
            search.Results = TempData["ResultList"] as List<Result>;
            ViewBag.resultIndex = Enumerable.Range(1, search.Results.Count)
                .Where(i => search.Results[i-1].URL.Contains(search.Keyword))
                .ToList();
            return View(search);
        }

        //public Result FindResult(Search search)
        //{
        //    Result result = new Result();
        //    string pre_url = PrepareURL(search);
        //    search.Page = 0;
        //    bool isEndPage = false;
        //    string regex = CONSTVALUE.GOOGLE_REGEX; //Will Change based on User-Agent Header
        //    int searchPage = GetPageLoopBySearchAmount(search.SearchAmount); //Page start from 0

        //    //List<Result> results = new List<Result>();

        //    for (int i = 0; i < searchPage; i++)
        //    {
        //        int urlPage = GetSearchPageByPageLoop(i);
        //        search.url = string.Format(pre_url, urlPage);
        //        search.Results.AddRange(GetResultlistAsync(GetHTMLComtent(search.url).Result, urlPage, regex).Result);
        //    }

        //    TempData["ResultList"] = search.Results;

        //    return result;
        //}

        private int GetPageLoopBySearchAmount(int searchAmount)
        {
            return searchAmount / 10 - 1;
        }

        private int GetSearchPageByPageLoop(int searchPage)
        {
            return searchPage * 10;
        }

        private async Task<Html> GetHTMLComtent(string url, int page)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                wc.Headers.Add("Content-Type", "text/plain;charset=UTF-8");

                string content = await wc.DownloadStringTaskAsync(new Uri(url));
                return new Html(){Content = content, Page = page};
            }
        }

        private string PrepareURL(Search search)
        {
            if (search.url.Contains("google.com.au"))
            {
                string pre_url = CONSTVALUE.GOOGLE_URL;
                search.SearchWord = search.SearchWord.Replace(" ", "+");
                search.Keyword = "www.infotrack.com.au";
                return string.Format(pre_url, search.SearchWord, "{0}");
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

        private void GetResultList(Search search, string regex)
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
        }

        private async Task<List<Result>> GetResultlistAsync(Html html, string regex)
        {
            List<Result> resultList = new List<Models.Result>();
            StringBuilder sb = new StringBuilder();
            Regex reg = new Regex(regex, RegexOptions.IgnoreCase);
            Match match = reg.Match(html.Content);
            while (match.Success)
            {
                string temp = match.Groups[2].Value.Replace("<b>", "").Replace("</b>", "");
                //int currentPage = html.Page / 10 + 1;
                resultList.Add(new Result() { Page = html.Page, URL = temp });
                match = match.NextMatch();
            }
            return resultList;
        }
    }
}