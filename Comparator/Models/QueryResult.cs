using System.Collections.Generic;

namespace Comparator.Models {
    public class QueryResult {
        public int ProcessedDataSets { get; set; }
        public double ObjATendency { get; set; }
        public double ObjBTendency { get; set; }
        public Dictionary<string, ClassifiedData> AspectResults { get; set; }
        public string Message { get; set; }
    }
}