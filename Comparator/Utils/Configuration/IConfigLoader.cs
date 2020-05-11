using Comparator.Utils.Monads;

namespace Comparator.Utils.Configuration {
    public interface IConfigLoader {
        Capsule<string> WatsonUrl { get; }
        Capsule<string> WatsonApiKey { get; }
        Capsule<string> KibanaUrl { get; }
        Capsule<string> KibanaUser { get; }
        Capsule<string> KibanaPassword { get; }
    }
}