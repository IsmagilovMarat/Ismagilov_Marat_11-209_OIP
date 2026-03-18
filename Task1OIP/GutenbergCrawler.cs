using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1OIP
{
    public class GutenbergCrawler
    {
        private readonly string _outputDir;
        private readonly HttpClient _httpClient;
        private readonly List<(string FileName, string Url)> _index;
        public GutenbergCrawler(string outputDir)
        {
            _outputDir = outputDir;
            _index = new List<(string, string)>();
            Directory.CreateDirectory(outputDir);

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Add("Accept",
                "text/plain, text/html, application/xhtml+xml, application/xml;q=0.9, */*;q=0.8");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
            _httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task CrawlAsync(string[] urls)
        {
            int downloaded = 0;
            int errors = 0;

            for (int i = 0; i < Math.Min(urls.Length, 100); i++)
            {
                string url = urls[i];
                string fileName = Path.GetFileName(url);

                string shortName = fileName.Length > 25 ? fileName.Substring(0, 22) + "..." : fileName.PadRight(25);

                try
                {
                    var response = await _httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();

                        if (!string.IsNullOrEmpty(content) && content.Length > 1000)
                        {
                            string localFile = $"выкачка_{downloaded + 1:D4}.txt";
                            string filePath = Path.Combine(_outputDir, localFile);

                            await File.WriteAllTextAsync(filePath, content, Encoding.UTF8);
                            _index.Add((localFile, url));
                            downloaded++;

                        }
                        else
                        {
                            errors++;
                        }
                    }
                    else
                    {
                        errors++;
                    }
                }
                catch (Exception ex)
                {
                    errors++;
                    Console.WriteLine($"Ошибки:{errors}");
                }

            }
            string indexPath = Path.Combine(_outputDir, "index.txt");
            await File.WriteAllLinesAsync(indexPath, _index.Select(e => $"{e.FileName}\t{e.Url}"));
        }
    }
}