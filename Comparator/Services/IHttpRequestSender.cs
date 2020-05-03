using System.Threading.Tasks;
using Comparator.Utils.Monads;

namespace Comparator.Services {
    public interface IHttpRequestSender {
        Task<Capsule<string>> GetAsString(string baseUri, string path);
        Task<Capsule<TReturn>> GetAs<TReturn>(string baseUri, string path);
        Task<Capsule<string>> PostAsString(string baseUri, string path, object content);
        Task<Capsule<TReturn>> PostAs<TReturn>(string baseUri, string path, object content);
    }
}