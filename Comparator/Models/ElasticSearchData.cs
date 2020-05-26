using System.Collections.Generic;

namespace Comparator.Models {
    public class ElasticSearchData {
        public IReadOnlyCollection<DepccDataSet> UnclassifiedData { get; set; }
        public ClassifiedData ClassifiedData { get; set; }
        public IEnumerable<ClassifiedData> ClassifiedTermData { get; set; }
        public int Count { get; set; }
    }
}