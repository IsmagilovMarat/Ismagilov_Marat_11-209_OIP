using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Task3OIP
{
    public class InvertedIndexBuilder
    {
        private readonly HashSet<string> _stopWords;
        private readonly Regex _validTokenRegex;

        public InvertedIndexBuilder()
        {
            _stopWords = new HashSet<string>(LoadStopWords(), StringComparer.OrdinalIgnoreCase);
            _validTokenRegex = new Regex(@"^[a-z]+('[a-z]+)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public async Task<Dictionary<string, HashSet<int>>> BuildIndexAsync(string docsDirectory)
        {
            var invertedIndex = new Dictionary<string, HashSet<int>>();
            var textFiles = Directory.GetFiles(docsDirectory, "*.txt")
                                     .Where(f => !f.Contains("index.txt"))
                                     .OrderBy(f => f)
                                     .ToList();

            for (int docId = 0; docId < textFiles.Count; docId++)
            {
                string filePath = textFiles[docId];
                string fileName = Path.GetFileName(filePath);

                try
                {
                    string content = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
                    var tokens = Tokenize(content);

                    var validTokens = FilterTokens(tokens);

                    var uniqueTokens = new HashSet<string>(validTokens, StringComparer.OrdinalIgnoreCase);

                    foreach (var token in uniqueTokens)
                    {
                        string lowerToken = token.ToLower();

                        if (!invertedIndex.ContainsKey(lowerToken))
                        {
                            invertedIndex[lowerToken] = new HashSet<int>();
                        }

                        invertedIndex[lowerToken].Add(docId + 1); 
                    }
                }
                catch 
                {
                    Console.WriteLine("ошибка");
                }
            }

            return invertedIndex.OrderBy(kv => kv.Key)
                                .ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        private List<string> Tokenize(string text)
        {
            var tokens = new List<string>();

            text = Regex.Replace(text, @"<[^>]+>", " ");
            text = Regex.Replace(text, @"\*\*\* START OF .*?\*\*\*", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\*\*\* END OF .*?\*\*\*", "", RegexOptions.IgnoreCase);

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

                filtered.Add(lowerToken);
            }

            return filtered;
        }

        private List<string> LoadStopWords()
        {
            return new List<string>
            {
                "a", "an", "the", "and", "or", "but", "if", "because", "as", "until", "while",
                "of", "at", "by", "for", "with", "about", "against", "between", "into", "through",
                "during", "before", "after", "above", "below", "to", "from", "up", "down", "in",
                "out", "on", "off", "over", "under", "again", "further", "then", "once", "here",
                "there", "when", "where", "why", "how", "all", "any", "both", "each", "few",
                "more", "most", "other", "some", "such", "no", "nor", "not", "only", "own",
                "same", "so", "than", "too", "very", "s", "t", "can", "will", "just", "don",
                "should", "now", "i", "me", "my", "myself", "we", "our", "ours", "ourselves",
                "you", "your", "yours", "yourself", "yourselves", "he", "him", "his", "himself",
                "she", "her", "hers", "herself", "it", "its", "itself", "they", "them", "their",
                "theirs", "themselves", "am", "is", "are", "was", "were", "be", "been", "being",
                "have", "has", "had", "having", "do", "does", "did", "doing"
            };
        }

        public async Task SaveIndexAsync(Dictionary<string, HashSet<int>> index, string filename)
        {
            using (var writer = new StreamWriter(filename, false, Encoding.UTF8))
            {
                await writer.WriteLineAsync("# Инвертированный индекс");
                await writer.WriteLineAsync($"# Всего терминов: {index.Count}");
                await writer.WriteLineAsync("# Формат: термин -> список документов\n");

                foreach (var kvp in index)
                {
                    string documents = string.Join(", ", kvp.Value.OrderBy(x => x));
                    await writer.WriteLineAsync($"{kvp.Key} -> {documents}");
                }
            }
        }
    }
}
