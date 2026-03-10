using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Task1OIP
{
    public class BooleanSearchEngine
    {
        private readonly Dictionary<string, HashSet<int>> _index;
        private readonly string _docsDirectory;
        private readonly List<string> _docFiles;

        public BooleanSearchEngine(Dictionary<string, HashSet<int>> index, string docsDirectory)
        {
            _index = index;
            _docsDirectory = docsDirectory;
            _docFiles = Directory.GetFiles(docsDirectory, "*.txt")
                                 .Where(f => !f.Contains("index.txt"))
                                 .OrderBy(f => f)
                                 .ToList();
        }

        public async Task RunAsync()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("БУЛЕВ ПОИСК ПО ИНВЕРТИРОВАННОМУ ИНДЕКСУ");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine("Доступные операторы:");
            Console.WriteLine("  AND - логическое И (оба термина должны быть)");
            Console.WriteLine("  OR  - логическое ИЛИ (хотя бы один термин)");
            Console.WriteLine("  NOT - логическое НЕ (исключить термин)");
            Console.WriteLine("\nПримеры запросов:");
            Console.WriteLine("  love AND death");
            Console.WriteLine("  (love AND death) OR (war AND peace)");
            Console.WriteLine("  love NOT hate");
            Console.WriteLine("\nДля выхода введите 'exit' или 'quit'\n");

            while (true)
            {
                Console.Write("Введите запрос: ");
                string? query = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(query) || query == "exit" || query == "quit")
                    break;

                try
                {
                    var parser = new QueryParser();
                    var expression = parser.Parse(query);

                    var result = EvaluateExpression(expression);

                    await DisplayResultsAsync(query, result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка в запросе: {ex.Message}");
                }

                Console.WriteLine();
            }
        }

        private HashSet<int> EvaluateExpression(QueryExpression expr)
        {
            if (expr is TermExpression termExpr)
            {
                string term = termExpr.Term.ToLower();
                return _index.TryGetValue(term, out var docs)
                    ? new HashSet<int>(docs)
                    : new HashSet<int>();
            }

            if (expr is NotExpression notExpr)
            {
                var subResult = EvaluateExpression(notExpr.Expression);
                var allDocs = Enumerable.Range(1, _docFiles.Count).ToHashSet();
                allDocs.ExceptWith(subResult);
                return allDocs;
            }

            if (expr is BinaryExpression binaryExpr)
            {
                var left = EvaluateExpression(binaryExpr.Left);
                var right = EvaluateExpression(binaryExpr.Right);

                return binaryExpr.Operator switch
                {
                    "AND" => new HashSet<int>(left.Intersect(right)),
                    "OR" => new HashSet<int>(left.Union(right)),
                    _ => throw new InvalidOperationException($"Неизвестный оператор: {binaryExpr.Operator}")
                };
            }

            throw new InvalidOperationException("Неизвестный тип выражения");
        }

        private async Task DisplayResultsAsync(string query, HashSet<int> docIds)
        {
            Console.WriteLine($"\nРезультаты поиска для: '{query}'");
            Console.WriteLine(new string('-', 50));

            if (docIds.Count == 0)
            {
                Console.WriteLine("Документы не найдены.");
                return;
            }

            Console.WriteLine($"Найдено документов: {docIds.Count}\n");

            // Показываем первые 10 документов с превью
            int count = 0;
            foreach (int docId in docIds.OrderBy(x => x))
            {
                if (count >= 10)
                {
                    Console.WriteLine($"... и ещё {docIds.Count - 10} документов");
                    break;
                }

                string fileName = _docFiles[docId - 1];
                string preview = await GetDocumentPreviewAsync(fileName);

                Console.WriteLine($"[{docId}] {Path.GetFileName(fileName)}");
                Console.WriteLine($"      Превью: {preview}\n");

                count++;
            }
        }

        private async Task<string> GetDocumentPreviewAsync(string filePath)
        {
            try
            {
                string content = await File.ReadAllTextAsync(filePath, Encoding.UTF8);

                // Берем первые 200 символов, удаляем лишние пробелы
                content = Regex.Replace(content, @"\s+", " ");
                return content.Length > 200 ? content.Substring(0, 197) + "..." : content;
            }
            catch
            {
                return "Не удалось загрузить превью";
            }
        }
    }
}
