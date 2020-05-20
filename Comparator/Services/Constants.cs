using System.Collections.Generic;
using System.Linq;

namespace Comparator.Services {
    public static class Constants {
        public static IEnumerable<string> Negations { get; } = new []{
            "not",
            "isn't",
            "can't",
            "cannot",
            "couldn't",
            "shouldn't",
            "ain't",
            "won't",
            "aren't",
            "don't",
            "doesn't",
            "wouldn't",
            "didn't",
            "less"
        };

        public static IEnumerable<string> PosComparativeAdjectives { get; } = new []{
            "better",
            "faster",
            "stronger",
            "thinner",
            "safer",
            "easier",
            "quieter",
            "cheaper",
            "nicer",
            "cooler",
            "quicker",
            "simpler",
            "lighter",
            "slimmer",
            "prettier",
            "handier",
            "superior",
            "greater",
            "speedier",
            "superb",
            "top notch",
            "fancier",
            "efficient",
            "attractive",
            "intuitive",
            "accessible",
            "newer",
            "sturdier",
            "robust"
        };

        public static IEnumerable<string> NegComparativeAdjectives { get; } = new []{
            "uglier",
            "slower",
            "inferior",
            "heavier",
            "inefficient",
            "unattractive",
            "bulkier",
            "louder",
            "noisier",
            "unsafe",
            "weaker",
            "difficult",
            "expensive",
            "pricier",
            "thicker",
            "worse",
            "unintuitive",
            "inaccessible",
            "older"
        };

        public static IEnumerable<string> PosAndNegComparativeAdjectives => PosComparativeAdjectives.Concat(NegComparativeAdjectives);
    }
}