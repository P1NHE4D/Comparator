using System;
using System.IO;
using Comparator.Utils.Monads;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;

namespace Comparator.Utils.Configuration {
    public class ConfigLoader : IConfigLoader {
        // Watson
        public Capsule<string> WatsonUrl => _config.Map(r => r["watson"]["url"].ToString());
        public Capsule<string> WatsonApiKey => _config.Map(r => r["watson"]["apikey"].ToString());
        
        // Elastic Search
        public Capsule<string> EsUrl => _config.Map(r => r["ElasticSearch"]["url"].ToString());
        public Capsule<string> EsUser => _config.Map(r => r["ElasticSearch"]["user"].ToString());
        public Capsule<string> EsPassword => _config.Map(r => r["ElasticSearch"]["password"].ToString());
        public Capsule<string> EsDefaultIndex => _config.Map(r => r["ElasticSearch"]["defaultIndex"].ToString());
        
        
        private readonly Capsule<JObject> _config;

        public ConfigLoader(IHostEnvironment env) {
            var filepath = env.IsDevelopment() ? "config.json" : "/etc/comparator/config.json";
            try {
                var rawJson = File.ReadAllText(filepath);
                _config = new Success<JObject>(JObject.Parse(rawJson));
            }
            catch (Exception e) {
                _config = new Failure<JObject>($"Error loading config file: {e}");
            }
        }
    }
}
