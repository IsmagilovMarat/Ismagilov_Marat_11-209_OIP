using Task1OIP;

namespace Task2OIP
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string inputDirectory = "1_Задание_Index+Страницы";
            string outputDirectory = "токены_леммы";
            var processor = new TextProcessor(inputDirectory, outputDirectory);
            await processor.ProcessAllFilesAsync();
        }
    }
}
