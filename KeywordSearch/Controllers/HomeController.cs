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
            //search.SearchWord = "online title search";
            search.SearchWord = "thankq";
            search.url = "https://www.google.com.au";
            return View(search);
        }

        [HttpPost]
        public async Task<ActionResult> Index(Search search)
        {
            string pre_url = PrepareURL(search);
            search.SearchAmount = 100;
            search.Page = 0;
            string regex = CONSTVALUE.GOOGLE_REGEX; //Will Change based on User-Agent Header
            int searchPage = GetPageLoopBySearchAmount(search.SearchAmount); //Page start from 0
            List<Task<Search>> tempSearchListTask = new List<Task<Search>>();

            for (int i = 0; i <= searchPage; i++)
            {
                int urlPage = GetSearchPageByPageLoop(i);
                search.url = string.Format(pre_url, urlPage);
                tempSearchListTask.Add(GetSearchComtent(search.url, i));
            }

            foreach (Task<Search> sr in tempSearchListTask)
            {
                Search tempSearch = await sr;
                GetResultList(tempSearch, regex);
                search.Results.AddRange(tempSearch.TempResults);
            }

            TempData["ResultList"] = search.Results;

            return RedirectToAction("Result", search);
        }

        //[HttpGet]
        //public async Task<ActionResult> AjaxCall(Search search)
        //{
        //    string pre_url = PrepareURL(search);
        //    search.SearchAmount = 30;
        //    search.Page = 0;
        //    string regex = CONSTVALUE.GOOGLE_REGEX; //Will Change based on User-Agent Header
        //    int searchPage = GetPageLoopBySearchAmount(search.SearchAmount); //Page start from 0
        //    List<Task<Search>> tempSearchListTask = new List<Task<Search>>();
            
        //    for (int i = 0; i <= searchPage; i++)
        //    {
        //        int urlPage = GetSearchPageByPageLoop(i);
        //        search.url = string.Format(pre_url, urlPage);
        //        tempSearchListTask.Add(GetSearchComtent(search.url, i));
        //    }

        //    foreach (Task<Search> sr in tempSearchListTask)
        //    {
        //        Search tempSearch = await sr;
        //        GetResultList(tempSearch, regex);
        //        search.Results.AddRange(tempSearch.TempResults);
        //    }

        //    TempData["ResultList"] = search.Results;

        //    return RedirectToAction("Result", search);
        //}

        [HttpGet]
        public ActionResult Result(Search search)
        {
            search.Results = TempData["ResultList"] as List<Result>;
            ViewBag.resultIndex = Enumerable.Range(0, search.Results.Count-1)
                .Where(i => search.Results[i].URL.ToUpper().Contains(search.Keyword.ToUpper()))
                .ToList();
            return View(search);
        }

        private int GetPageLoopBySearchAmount(int searchAmount)
        {
            return searchAmount / 10 - 1;
        }

        private int GetSearchPageByPageLoop(int searchPage)
        {
            return searchPage * 10;
        }

        private async Task<Search> GetSearchComtent(string url, int page)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("User-Agent", CONSTVALUE.USER_AGENT);
                wc.Headers.Add("Content-Type", "text/plain;charset=UTF-8");

                string content = await wc.DownloadStringTaskAsync(new Uri(url));
                return new Search(){Result = content, Page = page};
            }
        }

        private string PrepareURL(Search search)
        {
            if (search.url.Contains("google.com.au"))
            {
                string pre_url = CONSTVALUE.GOOGLE_URL;
                search.SearchWord = search.SearchWord.Replace(" ", "+");
                //search.Keyword = "www.infotrack.com.au";
                search.Keyword = "thankq";
                return string.Format(pre_url, search.SearchWord, "{0}");
            }else
            {
                return "";
            }
        }

        private void GetResultList(Search search, string regex)
        {
            search.TempResults = new List<Result>();
            int currentPageIndex = 0;
            StringBuilder sb = new StringBuilder();
            Regex reg = new Regex(regex, RegexOptions.IgnoreCase);
            Match match = reg.Match(search.Result);
            while (match.Success)
            {
                currentPageIndex++;
                string temp = match.Groups[2].Value.Replace("<b>", "").Replace("</b>", "");
                //int currentPage = search.Page / 10 + 1;
                search.TempResults.Add(new Result() { Page = search.Page, URL = temp, Index = currentPageIndex });
                match = match.NextMatch();
            }
        }
    }
}