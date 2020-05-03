using System.Threading.Tasks;
using Comparator.Utils.Monads;

namespace Comparator.OutgoingConnection
{
    public interface IHttpRequestSender
    {
        Task<Capsule<string>> GetAsString(string path);
        Task<Capsule<TReturn>> GetAs<TReturn>(string path);
        Task<Capsule<string>> PostAsString(string path, object content);
        Task<Capsule<TReturn>> PostAs<TReturn>(string path, object content);
    }
}