using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            // Загружаем стоп-слова (предлоги, союзы, местоимения и т.д.)
            _stopWords = new HashSet<string>(LoadStopWords(), StringComparer.OrdinalIgnoreCase);

            // Регулярное выражение для валидных токенов:
            // - только буквы (a-z)
            // - минимум 2 символа
            // - может содержать апостроф для сокращений (don't, it's)
            _validTokenRegex = new Regex(@"^[a-z]+('[a-z]+)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        private List<string> LoadStopWords()
        {
            return new List<string>
            {
                // Предлоги
                "about", "above", "across", "after", "against", "along", "among", "around", "at",
                "before", "behind", "below", "beneath", "beside", "between", "beyond", "but", "by",
                "down", "during", "except", "for", "from", "in", "inside", "into", "like", "near",
                "of", "off", "on", "onto", "out", "outside", "over", "past", "since", "through",
                "throughout", "to", "toward", "under", "underneath", "until", "up", "upon", "with",
                "within", "without",
                
                // Союзы
                "and", "or", "nor", "but", "yet", "so", "for", "because", "since", "as", "although",
                "though", "while", "whereas", "unless", "until", "if", "even", "whether",
                
                // Местоимения
                "i", "you", "he", "she", "it", "we", "they", "me", "him", "her", "us", "them",
                "my", "your", "his", "its", "our", "their", "mine", "yours", "hers", "ours", "theirs",
                "myself", "yourself", "himself", "herself", "itself", "ourselves", "yourselves", "themselves",
                "this", "that", "these", "those", "who", "whom", "which", "what", "whose", "whoever",
                "whatever", "whichever",
                
                // Артикли
                "a", "an", "the",
                
                // Вспомогательные глаголы
                "am", "is", "are", "was", "were", "be", "been", "being", "have", "has", "had",
                "having", "do", "does", "did", "doing", "will", "would", "shall", "should", "may",
                "might", "must", "can", "could",
                
                // Часто используемые слова
                "very", "too", "so", "such", "just", "only", "now", "then", "here", "there",
                "when", "where", "why", "how", "all", "any", "both", "each", "few", "more",
                "most", "other", "some", "such", "no", "nor", "not", "only", "own", "same",
                "than", "too", "very", "s", "t", "ll", "re", "ve", "don", "doesn", "didn",
                "isn", "aren", "wasn", "weren", "hasn", "haven", "hadn", "won", "wouldn",
                "shan", "shouldn", "can't", "couldn't", "mightn", "mustn't",
                
                // Числительные
                "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten",
                "hundred", "thousand", "million", "billion", "first", "second", "third", "fourth",
                "fifth", "sixth", "seventh", "eighth", "ninth", "tenth"
            };
        }

        public async Task ProcessAllFilesAsync()
        {
            string relativePath = "\\Task1OIP\\bin\\Debug\\net8.0\\1_Задание_Index+Страницы";
            string absolutePath =  Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", ".."));
            string oldPath = absolutePath+ relativePath;

            string goalPath = Directory.GetCurrentDirectory();

            Directory.CreateDirectory(goalPath+"/ВыгрузкаИз1Задания");
            string newPath = goalPath + "/ВыгрузкаИз1Задания";
            string[] allOldFiles = Directory.GetFiles(oldPath);
            allOldFiles = allOldFiles.Skip(1).ToArray();
            string filePath = Path.Combine(newPath, "выкачка");
            

            for (int i = 0; i < allOldFiles.Length; i++)
            {
                File.WriteAllText(filePath+i,"");
                string[] allNewFiles = Directory.GetFiles(newPath);


                File.Copy(allOldFiles[i], allNewFiles[i], overwrite: true);
            }
            

            var textFiles = Directory.GetFiles(@$"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName}/Task1OIP/Task1OIP/bin/Debug/net8.0/1_Задание_Index + Страницы,"+"*.txt")
                                     .Where(f => !f.Contains("index.txt"))
                                     .OrderBy(f => f)
                                     .ToList();

            Console.WriteLine($"Найдено {textFiles.Count} текстовых файлов для обработки\n");

            var allTokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var lemmatizer = new SimpleLemmatizer();

            foreach (var file in textFiles)
            {
                string fileName = Path.GetFileName(file);
                Console.Write($"Обработка {fileName}... ");

                try
                {
                    string content = await File.ReadAllTextAsync(file, Encoding.UTF8);

                    // Токенизация
                    var tokens = Tokenize(content);

                    // Фильтрация токенов
                    var validTokens = FilterTokens(tokens);

                    // Добавляем в общий список
                    foreach (var token in validTokens)
                    {
                        allTokens.Add(token.ToLower());
                    }

                    Console.WriteLine($"✓ найдено {validTokens.Count} валидных токенов");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ ошибка: {ex.Message}");
                }
            }

            Console.WriteLine($"\nВсего уникальных токенов после фильтрации: {allTokens.Count}");

            // Сохраняем список токенов
            string tokensPath = Path.Combine(_outputDir, "tokens.txt");
            var sortedTokens = allTokens.OrderBy(t => t).ToList();
            await File.WriteAllLinesAsync(tokensPath, sortedTokens);
            Console.WriteLine($"Список токенов сохранён: {tokensPath}");

            // Лемматизация
            Console.WriteLine("\nНачинаем лемматизацию...");
            var lemmas = lemmatizer.LemmatizeTokens(sortedTokens);

            // Сохраняем лемматизированные токены
            string lemmasPath = Path.Combine(_outputDir, "lemmas.txt");
            using (var writer = new StreamWriter(lemmasPath, false, Encoding.UTF8))
            {
                foreach (var lemma in lemmas.OrderBy(l => l.Key))
                {
                    string line = $"{lemma.Key} {string.Join(" ", lemma.Value)}";
                    await writer.WriteLineAsync(line);
                }
            }

            Console.WriteLine($"Лемматизированные токены сохранены: {lemmasPath}");

            // Статистика
            Console.WriteLine($"\n{new string('=', 50)}");
            Console.WriteLine($"Статистика обработки:");
            Console.WriteLine($"  Всего файлов: {textFiles.Count}");
            Console.WriteLine($"  Уникальных токенов: {allTokens.Count}");
            Console.WriteLine($"  Уникальных лемм: {lemmas.Count}");
            Console.WriteLine($"  Примеры токенов: {string.Join(", ", sortedTokens.Take(10))}");
        }

        private List<string> Tokenize(string text)
        {
            var tokens = new List<string>();

            // Удаляем HTML-разметку (Project Gutenberg часто добавляет HTML)
            text = Regex.Replace(text, @"<[^>]+>", " ");

            // Удаляем метаданные Gutenberg
            text = Regex.Replace(text, @"\*\*\* START OF (THIS|THE) PROJECT GUTENBERG EBOOK.*?\*\*\*", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\*\*\* END OF (THIS|THE) PROJECT GUTENBERG EBOOK.*?\*\*\*", "", RegexOptions.IgnoreCase);

            // Разбиваем на слова (учитываем апострофы для сокращений)
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

                // Проверка на минимальную длину
                if (lowerToken.Length < 2)
                    continue;

                // Проверка на стоп-слова
                if (_stopWords.Contains(lowerToken))
                    continue;

                // Проверка на валидность (только буквы, никаких цифр)
                if (!_validTokenRegex.IsMatch(lowerToken))
                    continue;

                // Проверка на повторяющиеся буквы (признак мусора)
                if (IsGarbage(lowerToken))
                    continue;

                filtered.Add(lowerToken);
            }

            return filtered;
        }

        private bool IsGarbage(string token)
        {
            // Слишком много повторяющихся букв (например, "aaaaaa")
            if (token.Distinct().Count() < 3 && token.Length > 4)
                return true;

            // Слишком много согласных подряд (например, "bcdfgh")
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

            return maxConsonants > 6; // Слишком много согласных подряд
        }
    }
}
