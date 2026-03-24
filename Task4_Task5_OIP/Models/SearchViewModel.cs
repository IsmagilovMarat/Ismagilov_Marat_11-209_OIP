using System;
using System.Collections.Generic;

namespace Task5_DEMO_OIP.Models
{
    public class SearchResult
    {
        public int DocumentId { get; set; }
        public string FileName { get; set; }
        public double Similarity { get; set; }
        public Dictionary<string, double> MatchingTerms { get; set; }

        public SearchResult()
        {
            MatchingTerms = new Dictionary<string, double>();
        }
    }

    public class SearchViewModel
    {
        public string Query { get; set; }
        public List<SearchResult> Results { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSearchPerformed { get; set; }
        public double SearchTime { get; set; }
        public int TotalDocuments { get; set; }
        public int UniqueTerms { get; set; }

        public SearchViewModel()
        {
            Results = new List<SearchResult>();
        }
    }
}