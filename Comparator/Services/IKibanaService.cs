using Comparator.Models;
using Comparator.Utils.Monads;

namespace Comparator.Services {
    public interface IKibanaService {
        Capsule<KibanaDataSet> FetchData(string keywords);
    }
}