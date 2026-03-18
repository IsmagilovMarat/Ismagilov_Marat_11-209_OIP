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
            string outputDirectory = "downloaded_english_pages";
            var crawler = new GutenbergCrawler(outputDirectory);
            await crawler.CrawlAsync(SitesUrls.urlsToDownload);


            //Task2
            //string inputDirectory = "downloaded_english_pages";
            //string outputDirectory = "processed_texts";

            //Console.WriteLine("Начинаем обработку английских текстов...");
            //Console.WriteLine($"Директория с файлами: {inputDirectory}");
            //Console.WriteLine($"Директория для результатов: {outputDirectory}\n");

            //var processor = new TextProcessor(inputDirectory, outputDirectory);
            //await processor.ProcessAllFilesAsync();

            //Task3 

            //string docsDirectory = "downloaded_english_pages";
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