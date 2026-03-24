using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Task5_DEMO_OIP;
using Task5_DEMO_OIP.Models;

namespace Task5_DEMO_OIP.Services
{
    public class SearchService
    {
        private List<DocumentVector> _documents;
        private Dictionary<string, double> _globalIdf;
        private Dictionary<string, List<int>> _invertedIndex;
        private readonly string _indexDir;
        private bool _isLoaded = false;

        public SearchService(string indexDir)
        {
            _indexDir = indexDir;
            _documents = new List<DocumentVector>();
            _globalIdf = new Dictionary<string, double>();
            _invertedIndex = new Dictionary<string, List<int>>();
        }

        public async Task LoadIndexAsync()
        {
            if (_isLoaded) return;

            string pattern = "tfidf_lemma_*.txt";

           
            string[] indexFiles = Directory.GetFiles(_indexDir, pattern)
                .OrderBy(f => f)
                .ToArray();

            if (indexFiles.Length == 0)
            {
                throw new FileNotFoundException($"Файлы индекса не найдены в директории: {_indexDir}");
            }

            _documents.Clear();
            _globalIdf.Clear();
            _invertedIndex.Clear();

            for (int i = 0; i < indexFiles.Length; i++)
            {
                var docVector = await LoadDocumentVectorAsync(indexFiles[i], i + 1);
                _documents.Add(docVector);

                foreach (var term in docVector.TermVectors.Keys)
                {
                    if (!_invertedIndex.ContainsKey(term))
                        _invertedIndex[term] = new List<int>();

                    _invertedIndex[term].Add(docVector.DocumentId);

                    if (!_globalIdf.ContainsKey(term))
                    {
                        _globalIdf[term] = await ExtractIdfFromFileAsync(indexFiles[i], term);
                    }
                }
            }

            _isLoaded = true;
        }

        private async Task<DocumentVector> LoadDocumentVectorAsync(string filePath, int docId)
        {
            var docVector = new DocumentVector(docId, Path.GetFileName(filePath));
            var lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8);

            foreach (var line in lines)
            {
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 3)
                {
                    string term = parts[0];
                    double idf = double.Parse(parts[1]);
                    double tfIdf = double.Parse(parts[2]);

                    docVector.TermVectors[term] = tfIdf;

                    if (!_globalIdf.ContainsKey(term))
                        _globalIdf[term] = idf;
                }
            }

            docVector.CalculateNorm();
            return docVector;
        }

        private async Task<double> ExtractIdfFromFileAsync(string filePath, string term)
        {
            var lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8);
            foreach (var line in lines)
            {
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 3 && parts[0] == term)
                {
                    return double.Parse(parts[1]);
                }
            }
            return 0;
        }

        private List<string> PreprocessQuery(string query)
        {
            query = query.ToLower();

            query = Regex.Replace(query, @"[^\w\s]", " ");

            var words = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var stopWords = new HashSet<string>
            {
                "и", "в", "на", "с", "по", "к", "у", "за", "из", "о", "об", "для",
                "а", "но", "или", "же", "бы", "что", "как", "так", "это", "все",
                "этот", "тот", "свой", "свои", "который", "быть", "the", "and", "of",
                "не", "да", "нет", "еще", "уже", "если", "потому", "когда", "где"
            };

            return words.Where(w => !stopWords.Contains(w) && w.Length > 1).ToList();
        }

        private Dictionary<string, double> ComputeQueryVector(List<string> queryTerms, int totalDocuments)
        {
            var queryVector = new Dictionary<string, double>();

            var termFreq = queryTerms.GroupBy(t => t)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var term in termFreq.Keys)
            {
                if (_globalIdf.ContainsKey(term))
                {
                    double tf = (double)termFreq[term] / queryTerms.Count;
                    double idf = _globalIdf[term];
                    double tfIdf = tf * idf;

                    queryVector[term] = tfIdf;
                }
            }

            return queryVector;
        }

        private double ComputeCosineSimilarity(Dictionary<string, double> queryVector, DocumentVector docVector)
        {
            double dotProduct = 0;

            foreach (var term in queryVector.Keys)
            {
                if (docVector.TermVectors.ContainsKey(term))
                {
                    dotProduct += queryVector[term] * docVector.TermVectors[term];
                }
            }

            double queryNorm = Math.Sqrt(queryVector.Values.Sum(v => v * v));

            if (queryNorm == 0 || docVector.VectorNorm == 0)
                return 0;

            return dotProduct / (queryNorm * docVector.VectorNorm);
        }

        public async Task<List<SearchResult>> SearchAsync(string query, int topK = 20)
        {
            if (!_isLoaded)
            {
                await LoadIndexAsync();
            }

            var queryTerms = PreprocessQuery(query);

            if (queryTerms.Count == 0)
            {
                return new List<SearchResult>();
            }

            var queryVector = ComputeQueryVector(queryTerms, _documents.Count);

            if (queryVector.Count == 0)
            {
                return new List<SearchResult>();
            }

            var candidateDocs = new HashSet<int>();
            foreach (var term in queryVector.Keys)
            {
                if (_invertedIndex.ContainsKey(term))
                {
                    foreach (var docId in _invertedIndex[term])
                    {
                        candidateDocs.Add(docId);
                    }
                }
            }

            var results = new List<SearchResult>();
            var docDict = _documents.ToDictionary(d => d.DocumentId);

            foreach (var docId in candidateDocs)
            {
                var doc = docDict[docId];
                double similarity = ComputeCosineSimilarity(queryVector, doc);

                if (similarity > 0)
                {
                    var result = new SearchResult
                    {
                        DocumentId = doc.DocumentId,
                        FileName = doc.FileName,
                        Similarity = similarity
                    };

                    foreach (var term in queryVector.Keys)
                    {
                        if (doc.TermVectors.ContainsKey(term))
                        {
                            result.MatchingTerms[term] = doc.TermVectors[term];
                        }
                    }

                    results.Add(result);
                }
            }

            return results.OrderByDescending(r => r.Similarity)
                .Take(topK)
                .ToList();
        }

        public int GetDocumentCount()
        {
            return _documents.Count;
        }

        public int GetUniqueTermsCount()
        {
            return _globalIdf.Count;
        }

        public bool IsLoaded()
        {
            return _isLoaded;
        }
    }
}