using Comparator.Utils.Monads;
using IBM.Watson.NaturalLanguageUnderstanding.v1.Model;

namespace Comparator.Services {
    public interface IWatsonService {
        Capsule<AnalysisResults> PerformNluAnalysis(Features features);
    }
}