using System.Collections.Generic;
using Comparator.Models;
using Comparator.Utils.Monads;

namespace Comparator.Services {
    public interface IDataAnalyser {
        Capsule<QueryResult> AnalyseQuery(string objA, string objB, IEnumerable<string> terms);
    }
}