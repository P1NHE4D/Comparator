using System.Collections.Generic;

namespace Comparator.Models {
    public class ElasticSearchData {
        public IReadOnlyCollection<DepccDataSet> UnclassifiedData { get; set; }
        public ICollection<string> ObjADataSet { get; set; }
        public ICollection<string> ObjBDataSet { get; set; }
        public int Count { get; set; }
    }
}