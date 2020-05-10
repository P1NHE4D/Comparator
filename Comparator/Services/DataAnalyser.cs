using System.Linq;
using Comparator.Models;
using Comparator.Utils.Monads;
using IBM.Watson.NaturalLanguageUnderstanding.v1.Model;

namespace Comparator.Services {
    public class DataAnalyser : IDataAnalyser {
        private readonly IKibanaService _kibana;
        private readonly IWatsonService _watson;

        public DataAnalyser(IKibanaService kibana, IWatsonService watson) {
            _kibana = kibana;
            _watson = watson;
        }

        /// <summary>
        /// Analyses the query using Kibana and Watson
        /// </summary>
        /// <param name="query">Query object containing information about the query</param>
        /// <returns>Returns a QueryResult object containing the results of the analysis</returns>
        public Capsule<QueryResult> AnalyseQuery(Query query) {
            var features = new Features() {
                Categories = new CategoriesOptions() { },
                //Concepts = new ConceptsOptions() {},
                Emotion = new EmotionOptions() {
                    Targets = query.Keywords.Split(' ').ToList()
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

            var result = new QueryResult();

            return _kibana.FetchData(query.Keywords)
                          .Bind(d => {
                              result.ProcessedDataSets = d.Count;
                              return _watson.AnalyseText(d.Data, features);
                          })
                          .Bind(ar => {
                              result.Results = ar;
                              return new Success<QueryResult>(result);
                          });
        }
    }
}