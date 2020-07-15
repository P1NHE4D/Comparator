using System;
using System.Collections.Generic;
using Comparator.Models;
using Comparator.Utils.Configuration;
using Comparator.Utils.Logger;
using Comparator.Utils.Monads;
using Elasticsearch.Net;
using Microsoft.AspNetCore.Http;
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
        /// <param name="aspects">user defined terms</param>
        /// <param name="quickSearch">enables quickSearch</param>
        /// <returns></returns>
        public Capsule<ElasticSearchData> FetchData(string objA, string objB, ICollection<string> aspects,
                                                    bool quickSearch) =>
            RequestData(objA, objB, aspects, quickSearch);

        private Capsule<ElasticSearchData> RequestData(string objA, string objB, ICollection<string> aspects,
                                                       bool quickSearch) {
            var query = new SearchDescriptor<DepccDataSet>();
            query.Size(quickSearch ? 1000 : 10000)
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

            return _client
                   .Map(c => c.Search<DepccDataSet>(query).Documents)
                   .Bind(data => data.Count <= 0
                                     ? Capsule<ElasticSearchData>.CreateFailure($"Status code: {StatusCodes.Status404NotFound}. No data found!", _logger)
                                     : new Success<ElasticSearchData>(new ElasticSearchData {
                                         UnclassifiedData = data,
                                         ClassifiedData = _classifier.ClassifyData(data, objA, objB),
                                         AspectData = aspects != null
                                                          ? _classifier.ClassifyAndSplitData(data, objA, objB, aspects)
                                                          : null
                                     }));
        }
    }
}