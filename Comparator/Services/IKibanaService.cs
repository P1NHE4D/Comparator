using Comparator.Utils.Monads;

namespace Comparator.Services {
    public interface IKibanaService {
        Capsule<string> FetchData(string keywords);
    }
}