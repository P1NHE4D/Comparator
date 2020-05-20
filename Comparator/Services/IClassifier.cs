using System.Collections.Generic;
using Comparator.Models;
using Comparator.Utils.Monads;
using Nest;

namespace Comparator.Services {
    public interface IClassifier {
        ElasticSearchData ClassifyData(ISearchResponse<DepccDataSet> data, string objA, string objB, IEnumerable<string> aspects);
    }
}