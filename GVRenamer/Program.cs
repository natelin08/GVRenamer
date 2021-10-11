using AngleSharp;
using AngleSharp.Dom;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Linq;

namespace GVRenamer
{
    internal class Program
    {
        static string FileExtensions = ".mp4,.avi,.rmvb,.wmv,.mov,.mkv,.flv,.ts,.webm,.iso,.mpg,.m4v";
        static string WebUrl = "https://md.gvdb.org/search-for/#gsc.tab=0&gsc.q={0}%20&gsc.sort=";
        static string OutputDir = "output";
        static string FailDir = "fail";
        static string DirectoryPath = Directory.GetCurrentDirectory();

        static void Main(string[] args)
        {
            string[] allfiles = Directory.GetFiles(DirectoryPath, "*.*", SearchOption.TopDirectoryOnly);
            allfiles = allfiles.Where(x => FileExtensions.Split(",").Contains(Path.GetExtension(x).ToLower())).ToArray();
            foreach (var item in allfiles)
            {
                string fileName = Path.GetFileName(item);
                string videoNumber = fileName.Substring(0, fileName.IndexOf("."));

                string link = GetSearchLink(videoNumber);
                string videoTitle = "";
                if (!string.IsNullOrWhiteSpace(link))
                {
                    videoTitle = GetVideoTitle(link, videoNumber);
                }
                MoveFile(item, videoTitle);
                if (!string.IsNullOrWhiteSpace(videoTitle))
                {
                    Console.WriteLine(videoNumber + " OK");
                }
            }
            Console.WriteLine("處理檔案: " + allfiles.Length);
            Console.WriteLine("請按任意鍵...");
            Console.ReadLine();
        }

        public static void MoveFile(string item, string videoTitle)
        {

            if (!string.IsNullOrWhiteSpace(videoTitle))
            {
                string outputFileDir = Path.Combine(DirectoryPath, OutputDir + "/" + videoTitle);
                Directory.CreateDirectory(outputFileDir);
                File.Move(item, outputFileDir + "/" + videoTitle + Path.GetExtension(item));
            }
            else
            {
                string outputFileDir = Path.Combine(DirectoryPath, FailDir);
                Directory.CreateDirectory(outputFileDir);
                File.Move(item, outputFileDir + "/" + Path.GetFileName(item));
            }
        }

        public static string GetSearchLink(string keyword)
        {
            string insideLink = "";

            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--headless");

            using (var driver = new ChromeDriver(".", options))
            {
                driver.Navigate().GoToUrl(String.Format(WebUrl, keyword));
                try
                {
                    insideLink = driver.FindElementByXPath("//*[@id='___gcse_0']/div/div/div/div[5]/div[2]/div/div/div[1]/div/div[1]/div[1]/div/a").GetAttribute("href");
                }
                catch
                {
                    try
                    {
                        insideLink = driver.FindElementByXPath("//*[@id='___gcse_0']/div/div/div/div[5]/div[2]/div/div/div[2]/div[1]/div[1]/div[1]/div/a").GetAttribute("href");
                    }
                    catch {
                        System.Console.WriteLine("查無資料: " + keyword);
                    }
                }
            }

            return insideLink;
        }


        public static string GetVideoTitle(string url, string videoNumber)
        {
            string fileName = "";
            var config = Configuration.Default.WithDefaultLoader();
            var dom = BrowsingContext.New(config).OpenAsync(url).Result;

            var brandTitle = dom.QuerySelectorAll("div.entry-content > p");
            var listCotentUl = dom.QuerySelectorAll("div.entry-content > ul");

            int index = -1;
            IElement queryItem = null;
            foreach (var ul in listCotentUl)
            {
                index++;
                var listCotentLi = ul.QuerySelectorAll("li");
                queryItem = listCotentLi.FirstOrDefault(x => x.TextContent.Contains(videoNumber));
                if (queryItem != null)
                {
                    break;
                }
            }

            if (queryItem != null)
            {
                string brand = "";

                int startIndex = brandTitle[0].TextContent.IndexOf("[ ") + 2;
                int endIndex = brandTitle[0].TextContent.IndexOf(" ]");

                if (endIndex < 0)
                {
                    endIndex = brandTitle[0].TextContent.IndexOf("]");
                }

                int length = endIndex - startIndex;
                if (startIndex < 0 || endIndex < 0 || length < 0)
                {
                    index++;
                }

                startIndex = brandTitle[index].TextContent.IndexOf("[") + 2;
                endIndex = brandTitle[index].TextContent.IndexOf(" ]");

                if (endIndex < 0)
                {
                    endIndex = brandTitle[index].TextContent.IndexOf("]");
                }

                length = endIndex - startIndex;

                brand = brandTitle[index].TextContent.Substring(startIndex, length);
                brand = brand.Replace("[email protected]", "G@MES");

                startIndex = queryItem.TextContent.IndexOf("] ") + 1;
                endIndex = queryItem.TextContent.IndexOf(" (");
                length = endIndex - startIndex;
                string videoTitle = "";

                if (length > 0)
                {
                    videoTitle = queryItem.TextContent.Substring(startIndex, length);
                }
                else
                {
                    videoTitle = queryItem.TextContent.Substring(startIndex);
                }

                if (videoTitle.Length > 40)
                {
                    videoTitle = videoTitle.Substring(0, 40) + "(...)";
                }

                fileName = string.Format("{0} [{1}] {2}", videoNumber, brand, videoTitle);
            }

            return fileName.Replace("  ", " ");
        }
    }
}
