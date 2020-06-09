using System.Collections.Generic;
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
        /// <param name="objA"></param>
        /// <param name="objB"></param>
        /// <param name="aspects"></param>
        /// <returns>Returns a QueryResult object containing the results of the analysis</returns>
        public Capsule<QueryResult> AnalyseQuery(string objA, string objB, IEnumerable<string> aspects) {
            var features = new Features() {
                Categories = new CategoriesOptions() { },
                //Concepts = new ConceptsOptions() {},
                Emotion = new EmotionOptions() {
                    Targets = new List<string>{objA, objB}
                },
                Keywords = new KeywordsOptions() {
                    Sentiment = true,
                    Emotion = true,
                    Limit = 20
                },
                //Relations =  new RelationsOptions() {},
                //SemanticRoles = new SemanticRolesOptions() {},
                Sentiment = new SentimentOptions() { }
            };

            return from d in _elasticSearch.FetchData(objA, objB, aspects)
                   from ar in _watson.AnalyseText(
                       string.Join(" ", d.ClassifiedData.ObjAData, d.ClassifiedData.ObjBData), features)
                   let prefersObjA = (double) d.ClassifiedData.ObjAData.Count /
                                     (d.ClassifiedData.ObjAData.Count + d.ClassifiedData.ObjBData.Count)
                   let prefersObjB = (double) d.ClassifiedData.ObjBData.Count /
                                     (d.ClassifiedData.ObjAData.Count + d.ClassifiedData.ObjBData.Count)
                   let termResults = d.ClassifiedTermData
                   select new QueryResult {
                       ProcessedDataSets = d.ClassifiedData.DataCount,
                       ObjATendency = d.ClassifiedData.ObjATendency,
                       ObjBTendency = d.ClassifiedData.ObjBTendency,
                       AspectResults = termResults
                   };
        }
    }
}