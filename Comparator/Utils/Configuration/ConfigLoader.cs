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
        
        // Kibana
        public Capsule<string> KibanaUrl => _config.Map(r => r["kibana"]["url"].ToString());
        public Capsule<string> KibanaUser => _config.Map(r => r["kibana"]["user"].ToString());
        public Capsule<string> KibanaPassword => _config.Map(r => r["kibana"]["password"].ToString());
        
        
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
