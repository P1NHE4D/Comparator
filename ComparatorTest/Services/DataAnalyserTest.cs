using Comparator.Models;
using Comparator.Services;
using Comparator.Utils.Monads;
using IBM.Watson.NaturalLanguageUnderstanding.v1.Model;
using Xunit;

namespace ComparatorTest.Services {
    public class DataAnalyserTest {
        private readonly DataAnalyser _analyser;

        public DataAnalyserTest() {
            IKibanaService kibana = new KibanaService();
            IWatsonService watson = new WatsonService();
            _analyser = new DataAnalyser(kibana, watson);
        }

        [Fact]
        public void TestAnalyse() {
            const string keywords = "Test";
            _analyser.AnalyseQuery(keywords).Access(innerValue => {
                Assert.Equal( keywords, innerValue.Results.AnalyzedText);
                Assert.Equal(5, innerValue.ProcessedDataSets);
            });
        }
    }

    public class KibanaService : IKibanaService {
        public Capsule<KibanaDataSet> FetchData(string keywords) {
            return new Success<KibanaDataSet>(new KibanaDataSet {
                Count = 5,
                Data = keywords
            });
        }
    }

    public class WatsonService : IWatsonService {

        public Capsule<AnalysisResults> AnalyseText(string text, Features features, string language = "en") {
            return new Success<AnalysisResults>(new AnalysisResults {
                AnalyzedText = text
            });
        }

        public Capsule<AnalysisResults> AnalyseUrl(string url, Features features, bool clean = false, string language = "en") {
            return new Success<AnalysisResults>(new AnalysisResults());
        }

        public Capsule<AnalysisResults> AnalyseHtml(string html, Features features, bool clean = false, string language = "en") {
            return new Success<AnalysisResults>(new AnalysisResults());
        }
    }
}