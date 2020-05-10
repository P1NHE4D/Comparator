using Comparator.Utils.Monads;

namespace Comparator.Services {
    public interface IConfigurationLoader {
        Capsule<string> WatsonUrl { get; }
        Capsule<string> WatsonUser { get; }
        Capsule<string> WatsonPassword { get; }
        Capsule<string> KibanaUrl { get; }
        Capsule<string> KibanaUser { get; }
        Capsule<string> KibanaPassword { get; }
    }
}