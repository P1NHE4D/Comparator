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
        public Capsule<ElasticSearchData> FetchData(string objA, string objB, IEnumerable<string> terms = null) {
            return RequestData(objA, objB, terms);
        }

        private Capsule<ElasticSearchData> RequestData(string objA, string objB, IEnumerable<string> terms = null) {
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

            return from c in _client
                   let d = c.Search<DepccDataSet>(searchQuery)
                   select new ElasticSearchData {
                       UnclassifiedData = d.Documents,
                       ClassifiedData = _classifier.ClassifyData(d, objA, objB),
                       ClassifiedTermData = _classifier.ClassifyAndSplitData(d, objA, objB, searchTerms)
                   };
        }
    }
}