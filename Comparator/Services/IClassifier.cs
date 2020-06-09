using System.Collections.Generic;
using Comparator.Models;

namespace Comparator.Services {
    public interface IClassifier {
        ClassifiedData ClassifyData(IEnumerable<DepccDataSet> data, string objA, string objB);

        Dictionary<string, ClassifiedData> ClassifyAndSplitData(IEnumerable<DepccDataSet> data, string objA, string objB,
                                                IEnumerable<string> aspects);
    }
}