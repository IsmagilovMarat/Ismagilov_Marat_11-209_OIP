using System.Text;
using System.Text.RegularExpressions;
using Task1OIP;
using Task1OIP.Data;

namespace EnglishTextCrawler
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string inputDirectory = "1_Задание_Index+Страницы";
            var crawler = new Crawler(inputDirectory);
            await crawler.CrawlAsync(SitesUrls.urlsToDownload);

        }
    }

}