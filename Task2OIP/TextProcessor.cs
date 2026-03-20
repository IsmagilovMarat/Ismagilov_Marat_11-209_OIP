using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Task1OIP
{
    public class TextProcessor
    {
        private readonly string _inputDir;
        private readonly string _outputDir;
        private readonly HashSet<string> _stopWords;
        private readonly Regex _validTokenRegex;

        public TextProcessor(string inputDir, string outputDir)
        {
            _inputDir = inputDir;
            _outputDir = outputDir;
            Directory.CreateDirectory(outputDir);

            _stopWords = new HashSet<string>(LoadStopWords(), StringComparer.OrdinalIgnoreCase);
            _validTokenRegex = new Regex(@"^[a-z]+('[a-z]+)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        private List<string> LoadStopWords()
        {
            return new List<string>
            {
                "about", "above", "across", "after", "against", "along", "among", "around", "at",
                "before", "behind", "below", "beneath", "beside", "between", "beyond", "but", "by",
                "down", "during", "except", "for", "from", "in", "inside", "into", "like", "near",
                "of", "off", "on", "onto", "out", "outside", "over", "past", "since", "through",
                "throughout", "to", "toward", "under", "underneath", "until", "up", "upon", "with",
                "within", "without", "and", "or", "nor", "but", "yet", "so", "for", "because",
                "since", "as", "although", "though", "while", "whereas", "unless", "until", "if",
                "even", "whether", "i", "you", "he", "she", "it", "we", "they", "me", "him", "her",
                "us", "them", "my", "your", "his", "its", "our", "their", "a", "an", "the", "am",
                "is", "are", "was", "were", "be", "been", "being", "have", "has", "had", "having",
                "do", "does", "did", "doing", "will", "would", "shall", "should", "may", "might",
                "must", "can", "could", "very", "too", "so", "such", "just", "only", "now", "then",
                "here", "there", "when", "where", "why", "how", "all", "any", "both", "each", "few",
                "more", "most", "other", "some", "no", "nor", "not", "own", "same", "than", "one",
                "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten"
            };
        }

        public async Task ProcessAllFilesAsync()
        {
            string oldPath = GetTask1Path();
            string newPath = Path.Combine(Directory.GetCurrentDirectory(), "ВыгрузкаИз1Задания");

            string[] allOldFiles = CopyFiles(oldPath, newPath);

            var lemmatizer = new SimpleLemmatizer();
            int fileNumber = 1;

            foreach (var file in allOldFiles)
            {
                string fileName = Path.GetFileName(file);

                try
                {
                    string content = await File.ReadAllTextAsync(file, Encoding.UTF8);

                    var tokens = Tokenize(content);
                    var validTokens = FilterTokens(tokens);

                    var uniqueTokens = validTokens.Distinct(StringComparer.OrdinalIgnoreCase)
                                                   .OrderBy(t => t)
                                                   .ToList();

                    var tokenToLemma = new Dictionary<string, string>();
                    foreach (var token in uniqueTokens)
                    {
                        string lemma = lemmatizer.GetLemma(token);
                        tokenToLemma[token] = lemma;
                    }

                    var lemmaGroups = new Dictionary<string, List<string>>();
                    foreach (var kvp in tokenToLemma)
                    {
                        if (!lemmaGroups.ContainsKey(kvp.Value))
                        {
                            lemmaGroups[kvp.Value] = new List<string>();
                        }
                        lemmaGroups[kvp.Value].Add(kvp.Key);
                    }

                    string tokensFile = Path.Combine(_outputDir, $"tokens_with_lemmas_{fileNumber:D4}.txt");
                    using (var writer = new StreamWriter(tokensFile, false, Encoding.UTF8))
                    {
                        foreach (var token in uniqueTokens)
                        {
                            await writer.WriteLineAsync($"{token}\t{tokenToLemma[token]}");
                        }
                    }

                    string lemmasFile = Path.Combine(_outputDir, $"lemmas_{fileNumber:D4}.txt");
                    using (var writer = new StreamWriter(lemmasFile, false, Encoding.UTF8))
                    {
                        foreach (var lemma in lemmaGroups.OrderBy(l => l.Key))
                        {
                            string line = $"{lemma.Key}: {string.Join(", ", lemma.Value.OrderBy(t => t))}";
                            await writer.WriteLineAsync(line);
                        }
                    }

                    fileNumber++;
                }
                catch 
                {
                    Console.WriteLine(" ошибка");
                }
            }
        }

        private string GetTask1Path()
        {
            string absolutePath = Path.GetFullPath(Path.Combine(
                Directory.GetCurrentDirectory(), "..", "..", "..", ".."));
            return Path.Combine(absolutePath, "Task1OIP", "bin", "Debug", "net8.0", "1_Задание_Index+Страницы");
        }

        private string[] CopyFiles(string sourcePath, string destPath)
        {
            if (!Directory.Exists(sourcePath))
            {
                throw new DirectoryNotFoundException($"Папка не найдена: {sourcePath}");
            }

            Directory.CreateDirectory(destPath);

            string[] files = Directory.GetFiles(sourcePath, "*.txt")
                .Where(f => !Path.GetFileName(f).Contains("index.txt"))
                .OrderBy(f => f)
                .ToArray();

            for (int i = 0; i < files.Length; i++)
            {
                string fileName = Path.GetFileName(files[i]);
                string destFile = Path.Combine(destPath, fileName);
                File.Copy(files[i], destFile, overwrite: true);

            }

            return Directory.GetFiles(destPath, "*.txt")
                .Where(f => !Path.GetFileName(f).Contains("index.txt"))
                .OrderBy(f => f)
                .ToArray();
        }

        private List<string> Tokenize(string text)
        {
            var tokens = new List<string>();

            text = Regex.Replace(text, @"<[^>]+>", " ");
            text = Regex.Replace(text, @"\*\*\* START OF (THIS|THE) PROJECT GUTENBERG EBOOK.*?\*\*\*", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\*\*\* END OF (THIS|THE) PROJECT GUTENBERG EBOOK.*?\*\*\*", "", RegexOptions.IgnoreCase);

            var matches = Regex.Matches(text, @"\b[a-zA-Z]+(?:'[a-zA-Z]+)?\b");

            foreach (Match match in matches)
            {
                tokens.Add(match.Value);
            }

            return tokens;
        }

        private List<string> FilterTokens(List<string> tokens)
        {
            var filtered = new List<string>();

            foreach (var token in tokens)
            {
                string lowerToken = token.ToLower();

                if (lowerToken.Length < 2)
                    continue;

                if (_stopWords.Contains(lowerToken))
                    continue;

                if (!_validTokenRegex.IsMatch(lowerToken))
                    continue;

                if (IsGarbage(lowerToken))
                    continue;

                filtered.Add(lowerToken);
            }

            return filtered;
        }

        private bool IsGarbage(string token)
        {
            if (token.Distinct().Count() < 3 && token.Length > 4)
                return true;

            var consonants = "bcdfghjklmnpqrstvwxyz";
            int maxConsonants = 0;
            int currentConsonants = 0;

            foreach (char c in token)
            {
                if (consonants.Contains(c))
                {
                    currentConsonants++;
                    maxConsonants = Math.Max(maxConsonants, currentConsonants);
                }
                else
                {
                    currentConsonants = 0;
                }
            }

            return maxConsonants > 6;
        }
    }
}