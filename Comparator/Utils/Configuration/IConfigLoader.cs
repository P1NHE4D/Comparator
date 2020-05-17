using Comparator.Utils.Monads;

namespace Comparator.Utils.Configuration {
    public interface IConfigLoader {
        Capsule<string> WatsonUrl { get; }
        Capsule<string> WatsonApiKey { get; }
        Capsule<string> EsUrl { get; }
        Capsule<string> EsUser { get; }
        Capsule<string> EsPassword { get; }
        Capsule<string> EsDefaultIndex { get; }
    }
}