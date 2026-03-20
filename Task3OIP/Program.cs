using Task3OIP;

namespace Task3OIP
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string absolutePath = Path.GetFullPath(Path.Combine(
               Directory.GetCurrentDirectory(), "..", "..", "..", ".."));

            string docsDirectory =  Path.Combine(absolutePath, "Task1OIP", "bin", "Debug", "net8.0", "1_Задание_Index+Страницы");
            string indexFile = "inverted_index.txt";

            var indexBuilder = new InvertedIndexBuilder();

            var invertedIndex = await indexBuilder.BuildIndexAsync(docsDirectory);

            await indexBuilder.SaveIndexAsync(invertedIndex, indexFile);

            var searchEngine = new BooleanSearchEngine(invertedIndex, docsDirectory);
            await searchEngine.RunAsync();
        }
    }
}
