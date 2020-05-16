using System;
using System.IO;
using System.Net;
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
        
        public Capsule<ElasticSearchData> FetchData(string keywords) {
            
            try {
                var webRequest =
                    WebRequest.Create("https://www.techrepublic.com/forums/discussions/linux-vs-windows-3/");
                using var response = webRequest.GetResponse();
                using var content = response.GetResponseStream();
                using var reader = new StreamReader(content ?? throw new NullReferenceException());
                return new Success<ElasticSearchData>(new ElasticSearchData() {
                    Data = reader.ReadToEnd(),
                    Count = 5400
                });
            }
            catch (Exception e) {
                return new Failure<ElasticSearchData>($"An error occurred {e.Message}");
            }
        }
    }
}