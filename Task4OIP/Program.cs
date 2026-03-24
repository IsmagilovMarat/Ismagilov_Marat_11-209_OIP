namespace Task4OIP
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string absolutePath = Path.GetFullPath(Path.Combine(
               Directory.GetCurrentDirectory(), "..", "..", "..", ".."));

            string tokensDir = Path.Combine(absolutePath, "Task2OIP", "bin", "Debug", "net8.0", "токены");
            string lemmaasDir = Path.Combine(absolutePath, "Task2OIP", "bin", "Debug", "net8.0", "токены_леммы");

            Directory.CreateDirectory("TF_IDF_Токены");
            Directory.CreateDirectory("TF_IDF_Леммы");

            string outputDirTokens = Path.Combine(Directory.GetCurrentDirectory(), "TF_IDF_Токены");
            string outputDirLemmas = Path.Combine(Directory.GetCurrentDirectory(), "TF_IDF_Леммы");

            var tfIdfToken = new TfIDf(tokensDir, outputDirTokens);
            await tfIdfToken.CalculateAndSaveAsync();

            var tfIdfLemma = new TfIDf(lemmaasDir, outputDirLemmas);
            await tfIdfLemma.CalculateAndSaveByLemmaAsync();

        }
    }
}
