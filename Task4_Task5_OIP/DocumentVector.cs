namespace Task5_DEMO_OIP
{
    public class DocumentVector
    {
        public int DocumentId { get; set; }
        public string FileName { get; set; }
        public Dictionary<string, double> TermVectors { get; set; }
        public double VectorNorm { get; set; }

        public DocumentVector(int id, string fileName)
        {
            DocumentId = id;
            FileName = fileName;
            TermVectors = new Dictionary<string, double>();
            VectorNorm = 0;
        }

        public void CalculateNorm()
        {
            VectorNorm = Math.Sqrt(TermVectors.Values.Sum(v => v * v));
        }
    }

}
