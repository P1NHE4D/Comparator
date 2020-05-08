using Comparator.Utils.Monads;
using IBM.Watson.NaturalLanguageUnderstanding.v1.Model;

namespace Comparator.Services {
    public interface IWatsonService {
        Capsule<AnalysisResults> AnalyseText(string text, Features features, string language = "en");
        Capsule<AnalysisResults> AnalyseUrl(string url, Features features, bool clean = false, string language = "en");
        Capsule<AnalysisResults> AnalyseHtml(string html, Features features, bool clean = false, string language = "en");
    }
}