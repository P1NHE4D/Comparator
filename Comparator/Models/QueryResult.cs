using IBM.Watson.NaturalLanguageUnderstanding.v1.Model;

namespace Comparator.Models {
    public class QueryResult {
        // TODO: Specify content
        public Query Query { get; set; }
        public int ProcessedDataSets { get; set; }
        public AnalysisResults Results { get; set; }
        public string Message { get; set; }
    }
}