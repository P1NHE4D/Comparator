using System.Collections.Generic;
using Comparator.Models;
using Comparator.Services;
using Comparator.Utils.Monads;
using IBM.Watson.NaturalLanguageUnderstanding.v1.Model;
using Xunit;

namespace ComparatorTest.Services {
    public class DataAnalyserTest {
        private readonly DataAnalyser _analyser;

        public DataAnalyserTest() {
            IElasticSearchService elasticSearch = new ElasticSearchService();
            IWatsonService watson = new WatsonService();
            _analyser = new DataAnalyser(elasticSearch, watson);
        }

        [Fact]
        public void TestAnalyse() {
            const string objA = "Test";
            const string objB = "Bla";
            _analyser.AnalyseQuery(objA, objB, new []{""}).Access(innerValue => {
                Assert.Equal( objA + objB, innerValue.Results.AnalyzedText);
                Assert.Equal(5, innerValue.ProcessedDataSets);
            });
        }
    }

    public class ElasticSearchService : IElasticSearchService {
        public Capsule<ElasticSearchData> FetchData(string keywords) {
            return new Success<ElasticSearchData>(new ElasticSearchData {
                Count = 5,
                Data = keywords
            });
        }

        public Capsule<ElasticSearchData> FetchData(string objA, string objB, IEnumerable<string> terms) {
            return new Success<ElasticSearchData>(new ElasticSearchData {
                Count = 5,
                Data = objA + objB
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