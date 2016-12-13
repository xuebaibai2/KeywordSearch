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
            UrlCheck(search);

            search.url += "/search";
            WebClient wc = new WebClient();
            NameValueCollection stringQuery = new NameValueCollection();
            stringQuery.Add("q", search.Keyword);
            wc.QueryString.Add(stringQuery);
            search.Result = wc.DownloadString(search.url).Replace("<!doctype html>", "<?xml version=\"1.0\" encoding=\"utf - 16\"?>");

            //search.Result = "<?xml version=\"1.0\" encoding=\"utf - 16\"?><html xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><head></head><body class=\"s\"></body></html>";

            //集齐所有排除项一并删除
            string regex_style = @"(<style>.*?<\/style>)";
            string regex_script = @"(<script\s?.*?>.*?<\/script>)";
            string regex_noscript = @"(<noscript>.*?<\/noscript>)";
            CleanResult(search, regex_style);
            CleanResult(search, regex_script);
            CleanResult(search, regex_noscript);
            var s = search.Result;

            Serializer serializer = new Serializer();
            Html html = serializer.GetDeserializedObj<Html>(search.Result);
            return View("Result", html);
            //return RedirectToAction("Result", search);
        }
        private void CleanResult(Search search, string regex)
        {
            List<string> resultList = new List<string>();
            Regex reg = new Regex(regex, RegexOptions.IgnoreCase);
            Match match = reg.Match(search.Result);
            while (match.Success)
            {
                resultList.Add(match.Groups[0].Value);
                match = match.NextMatch();
            }

            foreach (string str in resultList)
            {
                search.Result = search.Result.Replace(str, "");
            }
        }
        [HttpGet]
        public ActionResult Result(Html html)
        {

            //Serializer serializer = new Serializer();
            //Html html = serializer.GetDeserializedObj<Html>(search.Result);

            return View(html);
        }

        /// <summary>
        /// Check url integrity
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        private string UrlCheck(Search search)
        {
            return "";
        }
    }
}