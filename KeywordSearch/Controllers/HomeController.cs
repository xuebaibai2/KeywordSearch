using KeywordSearch.Models;
using KeywordSearch.Utility;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Net;
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
            search.Keyword = "online title search";
            search.SearchWord = "www.infotrack.com.au";
            return View(search);
        }

        [HttpPost]
        public async Task<ActionResult> Index(Search search)
        {
            string pre_url = PrepareURL(search);
            int searchPage = GetPageLoopBySearchAmount(CONSTVALUE.SEARCHAMOUNT); 
            List<Task<Search>> tempSearchListTask = new List<Task<Search>>();

            //Load html content to tempSearchList
            for (int i = 0; i <= searchPage; i++)
            {
                search.url = string.Format(pre_url, GetSearchPageByCurrentPage(i));
                tempSearchListTask.Add(GetSearchComtent(search.url, i));
            }

            //Use regular expression to find matching result
            foreach (Task<Search> sr in tempSearchListTask)
            {
                Search tempSearch = await sr;
                GetResultList(tempSearch, CONSTVALUE.GOOGLE_REGEX);
                search.Results.AddRange(tempSearch.TempResults);
            }

            TempData["ResultList"] = search.Results;

            return RedirectToAction("Result", search);
        }

        [HttpGet]
        public ActionResult Result(Search search)
        {
            search.Results = TempData["ResultList"] as List<Result>;
            ViewBag.resultIndex = Enumerable.Range(0, search.Results.Count - 1)
                .Where(i => search.Results[i].URL.ToUpper().Contains(search.SearchWord.ToUpper()))
                .ToList();
            return View(search);
        }

        #region Private Methods
        private int GetPageLoopBySearchAmount(int searchAmount)
        {
            return searchAmount / 10 - 1;
        }

        private int GetSearchPageByCurrentPage(int searchPage)
        {
            return searchPage * 10;
        }

        private async Task<Search> GetSearchComtent(string url, int page)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("User-Agent", CONSTVALUE.USER_AGENT);
                wc.Headers.Add("Content-Type", CONSTVALUE.CONTENT_TYPE);
                string content = await wc.DownloadStringTaskAsync(new Uri(url));
                return new Search() { Result = content, Page = page };
            }
        }

        private string PrepareURL(Search search)
        {
            search.url = CONSTVALUE.GOOGLE_URL;
            search.Keyword = search.Keyword.Replace(" ", "+");
            return string.Format(search.url, search.Keyword, "{0}");
        }

        private void GetResultList(Search search, string regex)
        {
            search.TempResults = new List<Result>();
            int currentPageIndex = 0;
            Regex reg = new Regex(regex, RegexOptions.IgnoreCase);
            Match match = reg.Match(search.Result);
            while (match.Success)
            {
                currentPageIndex++;
                string temp = match.Groups[2].Value.Replace("<b>", "").Replace("</b>", "");
                search.TempResults.Add(new Result() { Page = search.Page, URL = temp, Index = currentPageIndex });
                match = match.NextMatch();
            }
        }
        #endregion
    }
}