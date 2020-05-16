using System.Linq;
using Comparator.Models;
using Comparator.Utils.Monads;
using IBM.Watson.NaturalLanguageUnderstanding.v1.Model;

namespace Comparator.Services {
    public class DataAnalyser : IDataAnalyser {
        private readonly IElasticSearchService _elasticSearch;
        private readonly IWatsonService _watson;

        public DataAnalyser(IElasticSearchService elasticSearch, IWatsonService watson) {
            _elasticSearch = elasticSearch;
            _watson = watson;
        }

        /// <summary>
        /// Analyses the query using Kibana and Watson
        /// </summary>
        /// <param name="keywords">Query object containing information about the query</param>
        /// <returns>Returns a QueryResult object containing the results of the analysis</returns>
        public Capsule<QueryResult> AnalyseQuery(string keywords) {
            var features = new Features() {
                Categories = new CategoriesOptions() { },
                //Concepts = new ConceptsOptions() {},
                Emotion = new EmotionOptions() {
                    Targets = keywords.Split(' ').ToList()
                },
                Keywords = new KeywordsOptions() {
                    Sentiment = true,
                    Emotion = true,
                    Limit = 3
                },
                //Relations =  new RelationsOptions() {},
                //SemanticRoles = new SemanticRolesOptions() {},
                Sentiment = new SentimentOptions() { }
            };

            return from d in _elasticSearch.FetchData(keywords)
                   from ar in _watson.AnalyseText(d.Data, features)
                   select new QueryResult {ProcessedDataSets = d.Count, Results = ar};
        }
    }
}