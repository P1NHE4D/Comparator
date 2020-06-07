using System.Collections.Generic;

namespace Comparator.Models {
    public class ElasticSearchData {
        public IReadOnlyCollection<DepccDataSet> UnclassifiedData { get; set; }
        public ClassifiedData ClassifiedData { get; set; }
        public Dictionary<string, ClassifiedData> ClassifiedTermData { get; set; }
    }
}