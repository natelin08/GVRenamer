using System;
using System.Collections.Generic;
using System.Text;

namespace GVRenamer.Model
{

    public class CseModel
    {
        public string kind { get; set; }
        public Url url { get; set; }
        public Queries queries { get; set; }
        public Context context { get; set; }
        public Searchinformation searchInformation { get; set; }
        public Item[] items { get; set; }
    }

    public class Url
    {
        public string type { get; set; }
        public string template { get; set; }
    }

    public class Queries
    {
        public Request[] request { get; set; }
    }

    public class Request
    {
        public string title { get; set; }
        public string totalResults { get; set; }
        public string searchTerms { get; set; }
        public int count { get; set; }
        public int startIndex { get; set; }
        public string inputEncoding { get; set; }
        public string outputEncoding { get; set; }
        public string safe { get; set; }
        public string cx { get; set; }
    }

    public class Context
    {
        public string title { get; set; }
    }

    public class Searchinformation
    {
        public float searchTime { get; set; }
        public string formattedSearchTime { get; set; }
        public string totalResults { get; set; }
        public string formattedTotalResults { get; set; }
    }

    public class Item
    {
        public string kind { get; set; }
        public string title { get; set; }
        public string htmlTitle { get; set; }
        public string link { get; set; }
        public string displayLink { get; set; }
        public string snippet { get; set; }
        public string htmlSnippet { get; set; }
        public string formattedUrl { get; set; }
        public string htmlFormattedUrl { get; set; }
        public Pagemap pagemap { get; set; }
    }

    public class Pagemap
    {
        public Metatag[] metatags { get; set; }
        public Article[] article { get; set; }
    }

    public class Metatag
    {
        public string referrer { get; set; }
        public string themecolor { get; set; }
        public string thumbnail { get; set; }
        public string viewport { get; set; }
        public string rating { get; set; }
        public string google { get; set; }
        public string category { get; set; }
    }

    public class Article
    {
        public string datemodified { get; set; }
        public string text { get; set; }
        public string headline { get; set; }
    }

}
