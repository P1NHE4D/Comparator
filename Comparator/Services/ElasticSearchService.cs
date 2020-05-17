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

        private static readonly string[] Positives = {
            "cheaper", "better", "faster", "newer", "sturdier", "cooler", "easier"
        };

        private static readonly string[] Negatives = {
            "expensive", "slower", "older", "difficult", "uglier"
        };

        public ElasticSearchService(IConfigLoader config, ILoggerManager logger) {
            _logger = logger;
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
                   .Map(dataSet => new ElasticSearchData() {
                       Count = dataSet.Count,
                       Data = string.Join(" ", dataSet)
                   })
                   .Access(d => _logger.LogInfo(d.Data));
        }

        private Capsule<HashSet<string>> RequestData(string objA, string objB, IEnumerable<string> terms) {
            var searchQuery = new SearchDescriptor<DepccDataSet>();
            var searchTerms = terms ?? Positives.Concat(Negatives);
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
                          .Map(CleanData);
        }

        private static HashSet<string> CleanData(ISearchResponse<DepccDataSet> data) =>
            new HashSet<string>(from doc in data.Documents
                                where !(ContainsMarker(doc, Positives) && ContainsMarker(doc, Negatives))
                                where !IsQuestion(doc)
                                select doc.Text);

        private static bool ContainsMarker(DepccDataSet doc, IEnumerable<string> markers) =>
            markers.Any(doc.Text.Contains);

        private static bool IsQuestion(DepccDataSet doc) => doc.Text.Contains("?");
    }
}