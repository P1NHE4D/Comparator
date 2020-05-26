using System.Collections.Generic;
using Comparator.Models;
using Nest;

namespace Comparator.Services {
    public interface IClassifier {
        ClassifiedData ClassifyData(ISearchResponse<DepccDataSet> data, string objA, string objB);

        ISet<ClassifiedData> ClassifyAndSplitData(ISearchResponse<DepccDataSet> data, string objA, string objB,
                                                IEnumerable<string> terms);
    }
}