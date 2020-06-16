using System.Collections.Generic;
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
        /// <param name="objA"></param>
        /// <param name="objB"></param>
        /// <param name="aspects"></param>
        /// <returns>Returns a QueryResult object containing the results of the analysis</returns>
        public Capsule<QueryResult> AnalyseQuery(string objA, string objB, ICollection<string> aspects) {
            var features = new Features() {
                Emotion = new EmotionOptions {
                    Targets = aspects?.ToList(),
                    Document = true
                },
                Keywords = new KeywordsOptions {
                    Sentiment = true,
                    Limit = 5
                },
                Sentiment = new SentimentOptions {
                    Document = true,
                    Targets = aspects?.ToList()
                }
            };

            return from d in _elasticSearch.FetchData(objA, objB, aspects)
                   from arObjA in _watson.AnalyseText(
                       string.Join(" ", d.ClassifiedData.ObjAData), features)
                   from arObjB in _watson.AnalyseText(
                       string.Join(" ", d.ClassifiedData.ObjBData), features)
                   let prefersObjA = (double) d.ClassifiedData.ObjAData.Count /
                                     (d.ClassifiedData.ObjAData.Count + d.ClassifiedData.ObjBData.Count)
                   let prefersObjB = (double) d.ClassifiedData.ObjBData.Count /
                                     (d.ClassifiedData.ObjAData.Count + d.ClassifiedData.ObjBData.Count)
                   let aspectResults = d.AspectData
                   let objAEmotions = arObjA.Emotion?.Document?.Emotion
                   let objBEmotions = arObjB.Emotion?.Document?.Emotion
                   let objAKeywords = arObjA.Keywords?.Select(k => new Keyword {
                       Text = k.Text,
                       Relevance = k.Relevance,
                       Sentiment = k.Sentiment.Score,
                   })
                   let objBKeywords = arObjB.Keywords?.Select(k => new Keyword {
                       Text = k.Text,
                       Relevance = k.Relevance,
                       Sentiment = k.Sentiment.Score,
                   })
                   let objAAspectEmotions = arObjA.Emotion?.Targets?.ToDictionary(r => r.Text, r => r.Emotion)
                   let objBAspectEmotions = arObjB.Emotion?.Targets?.ToDictionary(r => r.Text, r => r.Emotion)
                   let objAAspectSentiment = arObjA.Sentiment?.Targets?.ToDictionary(r => r.Text, r => r.Score)
                   let objBAspectSentiment = arObjB.Sentiment?.Targets?.ToDictionary(r => r.Text, r => r.Score)
                   select new QueryResult {
                       Results = d.ClassifiedData,
                       ObjAEmotions = objAEmotions,
                       ObjBEmotions = objBEmotions,
                       ObjAKeywords = objAKeywords,
                       ObjBKeywords = objBKeywords,
                       ObjASentimentScore = arObjA.Sentiment?.Document?.Score,
                       ObjBSentimentScore = arObjB.Sentiment?.Document?.Score,
                       AspectResults = aspectResults,
                       ObjAAspectEmotions = objAAspectEmotions,
                       ObjBAspectEmotions = objBAspectEmotions,
                       ObjAAspectSentimentScores = objAAspectSentiment,
                       ObjBAspectSentimentScores = objBAspectSentiment
                   };
        }
    }
}