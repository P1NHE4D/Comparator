using System.Collections.Generic;
using Comparator.Models;
using Comparator.Utils.Monads;

namespace Comparator.Services {
    public interface IElasticSearchService {
        Capsule<ElasticSearchData> FetchData(string objA, string objB, IEnumerable<string> terms);
    }
}