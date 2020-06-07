using System;
using System.Collections.Generic;
using Comparator.Models;
using Comparator.Utils.Configuration;
using Comparator.Utils.Logger;
using Comparator.Utils.Monads;
using Elasticsearch.Net;
using Nest;

namespace Comparator.Services {
    public class ElasticSearchService : IElasticSearchService {
        private readonly ILoggerManager _logger;
        private readonly Capsule<ElasticClient> _client;
        private readonly IClassifier _classifier;

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

        /// <summary>
        /// Retrieves data from elastic search and classifies the data according to predefined and user defined terms
        /// </summary>
        /// <param name="objA">first object</param>
        /// <param name="objB">second object</param>
        /// <param name="terms">user defined terms</param>
        /// <returns></returns>
        public Capsule<ElasticSearchData> FetchData(string objA, string objB, IEnumerable<string> terms = null) =>
            RequestData(objA, objB, terms);

        /*
        private Capsule<ElasticSearchData> RequestData(string objA, string objB) {
            var searchQuery = new SearchDescriptor<DepccDataSet>();
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
                                               .Terms(Constants.PosAndNegComparativeAdjectives)));

            return from c in _client
                   let d = c.Search<DepccDataSet>(searchQuery)
                   select new ElasticSearchData {
                       UnclassifiedData = d.Documents,
                       ClassifiedData = _classifier.ClassifyData(d, objA, objB)
                   };
        }*/

        private Capsule<ElasticSearchData> RequestData(string objA, string objB, IEnumerable<string> terms) {
            var query = new SearchDescriptor<DepccDataSet>();
            query.Size(10000)
                      .Query(q =>
                                 q.Match(m => m
                                              .Field(f => f.Text)
                                              .Query(objA)) &&
                                 q.Match(m => m
                                              .Field(f => f.Text)
                                              .Query(objB)) &&
                                 q.Terms(t => t
                                              .Field(f => f.Text)
                                              .Terms(Constants.PosAndNegComparativeAdjectives)));
            return from c in _client
                   let data = c.Search<DepccDataSet>(query).Documents
                   select new ElasticSearchData {
                       UnclassifiedData = data,
                       ClassifiedData = _classifier.ClassifyData(data, objA, objB),
                       ClassifiedTermData = terms != null ?_classifier.ClassifyAndSplitData(data, objA, objB, terms) : null
                   };
        }
    }
}