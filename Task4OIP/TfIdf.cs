using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Task4OIP
{
    public class TfIDf
    {
        private readonly string _inputDir;
        private readonly string _outputDir;

        public TfIDf(string inputDir, string outputDir)
        {
            _inputDir = inputDir;
            _outputDir = outputDir;
            Directory.CreateDirectory(outputDir);
        }

        public async Task CalculateAndSaveAsync()
        {
            string[] tokenFiles = Directory.GetFiles(_inputDir, "tokens_*.txt")
                .OrderBy(f => f)
                .ToArray();

            var documentTerms = new List<HashSet<string>>();
            var allTerms = new HashSet<string>();

            foreach (var file in tokenFiles)
            {
                var terms = await LoadTermsFromFileAsync(file);
                documentTerms.Add(terms);

                foreach (var term in terms)
                {
                    allTerms.Add(term);
                }
            }

            int totalDocuments = documentTerms.Count;

            var termIdf = new Dictionary<string, double>();

            foreach (var term in allTerms)
            {
                int documentCount = documentTerms.Count(docTerms => docTerms.Contains(term));
                double idf = Math.Log((double)totalDocuments / documentCount);
                termIdf[term] = idf;
            }

            for (int i = 0; i < tokenFiles.Length; i++)
            {
                var tokenFile = tokenFiles[i];
                await ProcessDocumentAsync(tokenFile, termIdf, totalDocuments, i + 1);
            }

        }

        private async Task<HashSet<string>> LoadTermsFromFileAsync(string filePath)
        {
            var terms = new HashSet<string>();
            var lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8);

            foreach (var line in lines)
            {
                if (line.StartsWith("#"))
                    continue;

                var parts = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 1)
                {
                    string token = parts[0].Trim();
                    if (!string.IsNullOrEmpty(token))
                    {
                        terms.Add(token);
                    }
                }
            }

            return terms;
        }

        private async Task<Dictionary<string, int>> CalculateTermFrequenciesAsync(string filePath)
        {
            var termFrequencies = new Dictionary<string, int>();
            var lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8);

            foreach (var line in lines)
            {
                var parts = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 1)
                {
                    string token = parts[0].Trim();
                    if (!string.IsNullOrEmpty(token))
                    {
                        if (termFrequencies.ContainsKey(token))
                            termFrequencies[token]++;
                        else
                            termFrequencies[token] = 1;
                    }
                }
            }

            return termFrequencies;
        }

        private async Task ProcessDocumentAsync(string tokenFile, Dictionary<string, double> termIdf,
            int totalDocuments, int documentNumber)
        {
            var termFrequencies = await CalculateTermFrequenciesAsync(tokenFile);
            int totalTermsInDocument = termFrequencies.Values.Sum();

            var termTfIdf = new List<(string Term, double Tf, double Idf, double TfIdf)>();

            foreach (var kvp in termFrequencies.OrderBy(k => k.Key))
            {
                string term = kvp.Key;
                int termFrequency = kvp.Value;

                double tf = (double)termFrequency / totalTermsInDocument;

                double idf = termIdf.ContainsKey(term) ? termIdf[term] : 0;

                double tfIdf = tf * idf;

                termTfIdf.Add((term, tf, idf, tfIdf));
            }

            string outputFile = Path.Combine(_outputDir, $"tfidf_{documentNumber:D4}.txt");

            using (var writer = new StreamWriter(outputFile, false, Encoding.UTF8))
            {
                foreach (var item in termTfIdf)
                {
                    await writer.WriteLineAsync($"{item.Term} {item.Idf:F6} {item.TfIdf:F6}");
                }
            }
        }

        public async Task CalculateAndSaveByLemmaAsync()
        {
            string[] lemmaFiles = Directory.GetFiles(_inputDir, "tokens_with_lemmas_*.txt")
                .OrderBy(f => f)
                .ToArray();

            var documentLemmas = new List<HashSet<string>>();
            var allLemmas = new HashSet<string>();

            foreach (var file in lemmaFiles)
            {
                var lemmas = await LoadLemmasFromFileAsync(file);
                documentLemmas.Add(lemmas);

                foreach (var lemma in lemmas)
                {
                    allLemmas.Add(lemma);
                }
            }

            int totalDocuments = documentLemmas.Count;

            var lemmaIdf = new Dictionary<string, double>();

            foreach (var lemma in allLemmas)
            {
                int documentCount = documentLemmas.Count(docLemmas => docLemmas.Contains(lemma));
                double idf = Math.Log((double)totalDocuments / documentCount);
                lemmaIdf[lemma] = idf;
            }

            for (int i = 0; i < lemmaFiles.Length; i++)
            {
                var tokenFile = lemmaFiles[i];
                await ProcessDocumentByLemmaAsync(tokenFile, lemmaIdf, totalDocuments, i + 1);
            }
        }
        private async Task<HashSet<string>> LoadLemmasFromFileAsync(string filePath)
        {
            var lemmas = new HashSet<string>();
            var lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8);

            foreach (var line in lines)
            {
                var parts = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                {
                    string lemma = parts[1].Trim();
                    if (!string.IsNullOrEmpty(lemma))
                    {
                        lemmas.Add(lemma);
                    }
                }
            }
            return lemmas;
        }

        private async Task<Dictionary<string, int>> CalculateLemmaFrequenciesAsync(string filePath)
        {
            var lemmaFrequencies = new Dictionary<string, int>();
            var lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8);

            foreach (var line in lines)
            {
               
                var parts = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                {
                    string lemma = parts[1].Trim();
                    if (!string.IsNullOrEmpty(lemma))
                    {
                        if (lemmaFrequencies.ContainsKey(lemma))
                            lemmaFrequencies[lemma]++;
                        else
                            lemmaFrequencies[lemma] = 1;
                    }
                }
            }

            return lemmaFrequencies;
        }

        private async Task ProcessDocumentByLemmaAsync(string tokenFile, Dictionary<string, double> lemmaIdf,
            int totalDocuments, int documentNumber)
        {
            var lemmaFrequencies = await CalculateLemmaFrequenciesAsync(tokenFile);
            int totalLemmasInDocument = lemmaFrequencies.Values.Sum();

            var lemmaTfIdf = new List<(string Lemma, double Tf, double Idf, double TfIdf)>();

            foreach (var kvp in lemmaFrequencies.OrderBy(k => k.Key))
            {
                string lemma = kvp.Key;
                int lemmaFrequency = kvp.Value;

                double tf = (double)lemmaFrequency / totalLemmasInDocument;

                double idf = lemmaIdf.ContainsKey(lemma) ? lemmaIdf[lemma] : 0;

                double tfIdf = tf * idf;

                lemmaTfIdf.Add((lemma, tf, idf, tfIdf));
            }

            string outputFile = Path.Combine(_outputDir, $"tfidf_lemma_{documentNumber:D4}.txt");

            using (var writer = new StreamWriter(outputFile, false, Encoding.UTF8))
            {

                foreach (var item in lemmaTfIdf)
                {
                    await writer.WriteLineAsync($"{item.Lemma} {item.Idf:F6} {item.TfIdf:F6}");
                }
            }
        }
    }

}
