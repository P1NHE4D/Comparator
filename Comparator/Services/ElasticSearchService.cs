using System;
using System.Collections.Generic;
using System.Linq;
using Comparator.Models;
using Comparator.Utils.Configuration;
using Comparator.Utils.Logger;
using Comparator.Utils.Monads;
using Elasticsearch.Net;
using Nest;

namespace Comparator.Services {
    public class ElasticSearchService : IElasticSearchService {
        private ILoggerManager _logger;
        private Capsule<ElasticClient> _client;
        private IClassifier _classifier;

        public ElasticSearchService(IConfigLoader config, ILoggerManager logger, IClassifier classifier) {
            _logger = logger;
            _classifier = classifier;
            _client = from url in config.EsUrl
                      from user in config.EsUser
                      from password in config.EsPassword
                      from defaultIndex in config.EsDefaultIndex
                      let uri = new Uri(url)
                      let pool = new SingleNodeConnectionPool(uri)
                      select new ElasticClient(new ConnectionSettings(pool)
                                               .BasicAuthentication(user, password)
                                               .DefaultIndex(defaultIndex));
        }

        public Capsule<ElasticSearchData> FetchData(string objA, string objB, IEnumerable<string> terms) {
            return RequestData(objA, objB, terms)
                .Access(d => {
                    _logger.LogInfo("Prefers Object A: ");
                    foreach (var sentence in d.ObjADataSet) {
                        _logger.LogInfo(sentence);
                    }
                    _logger.LogInfo("##################");
                    _logger.LogInfo("Prefers Object B: ");
                    foreach (var sentence in d.ObjBDataSet) {
                        _logger.LogInfo(sentence);
                    }

                    var objAPercentage = (double) d.ObjADataSet.Count / d.Count * 100.00;
                    var objBPercentage = (double) d.ObjBDataSet.Count / d.Count * 100.00;
                    _logger.LogInfo($"Total number of documents: {d.Count}");
                    _logger.LogInfo($"Prefers Object A: {objAPercentage}%");
                    _logger.LogInfo($"Prefers Object B: {objBPercentage}%");
                });
        }

        private Capsule<ElasticSearchData> RequestData(string objA, string objB, IEnumerable<string> terms) {
            var searchQuery = new SearchDescriptor<DepccDataSet>();
            var searchTerms = terms ?? Constants.PosAndNegComparativeAdjectives;
            searchQuery.Size(10000)
                       .Query(q =>
                                  q.Match(m => m
                                               .Field(f => f.Text)
                                               .Query(objA)) &&
                                  q.Match(m => m
                                               .Field(f => f.Text)
                                               .Query(objB)) &&
                                  q.Terms(t => t
                                               .Field(f => f.Text)
                                               .Terms(searchTerms)));
            return _client.Map(c => c.Search<DepccDataSet>(searchQuery))
                          .Map(d => _classifier.ClassifyData(d, objA, objB, searchTerms));
        }
    }
}