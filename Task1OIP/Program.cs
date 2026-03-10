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
            //string outputDirectory = "downloaded_english_pages";
            //var crawler = new GutenbergCrawler(outputDirectory);
            //await crawler.CrawlAsync(SitesUrls.urlsToDownload);


            string inputDirectory = "downloaded_english_pages";
            string outputDirectory = "processed_texts";

            Console.WriteLine("Начинаем обработку английских текстов...");
            Console.WriteLine($"Директория с файлами: {inputDirectory}");
            Console.WriteLine($"Директория для результатов: {outputDirectory}\n");

            var processor = new TextProcessor(inputDirectory, outputDirectory);
            await processor.ProcessAllFilesAsync();


        }
    }


}