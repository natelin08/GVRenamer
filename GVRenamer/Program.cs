using AngleSharp;
using AngleSharp.Dom;
using GVRenamer.Extensions;
using GVRenamer.Model;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Linq;

namespace GVRenamer
{
    internal class Program
    {
        static string WebUrl = "https://md.gvdb.org/search-for/#gsc.tab=0&gsc.q={0}%20&gsc.sort=";
        static string DirectoryPath = Directory.GetCurrentDirectory();

        static void Main(string[] args)
        {
            string[] allfiles = GetFileListByFileExtensions();
            foreach (var file in allfiles)
            {
                string fileName = Path.GetFileName(file);
                string videoNumber = fileName.Substring(0, fileName.IndexOf("."));

                string link = GetSearchLink(videoNumber);
                VideoModel model = null;
                if (!string.IsNullOrWhiteSpace(link))
                {
                    model = GetVideoInfo(link, videoNumber);
                }
                MoveFile(file, model);
                if (model != null)
                {
                    Console.WriteLine(videoNumber + " OK");
                }
            }
            Console.WriteLine("處理檔案: " + allfiles.Length);
            Console.WriteLine("請按任意鍵...");
            Console.ReadLine();
        }

        public static string[] GetFileListByFileExtensions()
        {
            SearchOption searchOption = SearchOption.AllDirectories;

            if (!AppSettingsModel.GeneralSetting.SearchSubFolder)
            {
                searchOption = SearchOption.TopDirectoryOnly;
            }
            string[] fileList = Directory.GetFiles(DirectoryPath, "*.*", searchOption)
                .Where(x => AppSettingsModel.GeneralSetting.FileExtensions.Split(",").Contains(Path.GetExtension(x).ToLower())).ToArray();

            if (AppSettingsModel.GeneralSetting.SearchSubFolder && !string.IsNullOrWhiteSpace(AppSettingsModel.GeneralSetting.EscapeFolder))
            {
                foreach (var filePath in AppSettingsModel.GeneralSetting.EscapeFolder.Split(","))
                {
                    fileList = fileList.Where(x => !x.Contains(Path.DirectorySeparatorChar + filePath + Path.DirectorySeparatorChar)).ToArray();
                }
            }

            return fileList;
        }

        public static void MoveFile(string soureFilePath, VideoModel model)
        {
            if (model != null)
            {
                string videoTitle = AppSettingsModel.NameRuleSetting.NamingRule.ToRuleName(model);
                string folderName = AppSettingsModel.NameRuleSetting.FolderRule.ToRuleName(model);

                string outputFileDir = Path.Combine(DirectoryPath, AppSettingsModel.GeneralSetting.SuccessOutputFolder + folderName);
                Directory.CreateDirectory(outputFileDir);
                File.Move(soureFilePath, outputFileDir + "/" + videoTitle + Path.GetExtension(soureFilePath));
            }
            else if (AppSettingsModel.GeneralSetting.FailedMove)
            {
                string outputFileDir = Path.Combine(DirectoryPath, AppSettingsModel.GeneralSetting.FailedOutputFolder);
                Directory.CreateDirectory(outputFileDir);
                File.Move(soureFilePath, outputFileDir + "/" + Path.GetFileName(soureFilePath));
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
                    catch
                    {
                        Console.WriteLine("查無資料: " + keyword);
                    }
                }
            }
            return insideLink;
        }

        public static VideoModel GetVideoInfo(string url, string videoNumber)
        {
            VideoModel videoModel = null;
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
                videoModel = new VideoModel();
                string brand = "";
                #region 抓廠牌
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
                #endregion

                #region 抓片名
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

                if (videoTitle.Length > AppSettingsModel.NameRuleSetting.MaxTitleLength)
                {
                    videoTitle = videoTitle.Substring(0, AppSettingsModel.NameRuleSetting.MaxTitleLength) + AppSettingsModel.NameRuleSetting.MaxTitleOmitStr;
                }
                #endregion

                videoModel.Title = videoTitle.TrimIgnoreFileName();
                videoModel.Number = videoNumber;
                videoModel.Studio = brand.TrimIgnoreFileName();
            }

            return videoModel;
        }
    }
}
