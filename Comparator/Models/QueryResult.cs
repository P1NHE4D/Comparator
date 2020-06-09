using System.Collections.Generic;
using IBM.Watson.NaturalLanguageUnderstanding.v1.Model;

namespace Comparator.Models {
    public class QueryResult {
        public int ProcessedDataSets { get; set; }
        public double ObjATendency { get; set; }
        public double ObjBTendency { get; set; }
        public Dictionary<string, ClassifiedData> TermResults { get; set; }
        public string Message { get; set; }
    }
}