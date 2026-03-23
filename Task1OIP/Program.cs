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
            //Task1
            string inputDirectory = "1_Задание_Index+Страницы";
            var crawler = new Crawler(inputDirectory);
            await crawler.CrawlAsync(SitesUrls.urlsToDownload);


            //Task2


            //Task3 

            //string docsDirectory = "1_Задание_Index+Страницы";
            //string indexFile = "inverted_index.txt";

            //Console.WriteLine("Создание инвертированного индекса...\n");

            //var indexBuilder = new InvertedIndexBuilder();

            //// Строим индекс
            //var invertedIndex = await indexBuilder.BuildIndexAsync(docsDirectory);

            //// Сохраняем индекс в файл
            //await indexBuilder.SaveIndexAsync(invertedIndex, indexFile);

            //Console.WriteLine($"\nИндекс сохранён в файл: {indexFile}");
            //Console.WriteLine($"Всего терминов в индексе: {invertedIndex.Count}");

            //// Запускаем поисковый движок
            //var searchEngine = new BooleanSearchEngine(invertedIndex, docsDirectory);
            //await searchEngine.RunAsync();

        }
    }

}