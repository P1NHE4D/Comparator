using System;
using System.IO;
using Comparator.Utils.Monads;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace Comparator.Services {
    public class ConfigLoader : IConfigLoader {
        // Watson
        public Capsule<string> WatsonUrl => _config.Map(r => r["watson"]["url"].ToString());
        public Capsule<string> WatsonApiKey => _config.Map(r => r["watson"]["apikey"].ToString());
        
        // Kibana
        public Capsule<string> KibanaUrl => _config.Map(r => r["kibana"]["url"].ToString());
        public Capsule<string> KibanaUser => _config.Map(r => r["kibana"]["user"].ToString());
        public Capsule<string> KibanaPassword => _config.Map(r => r["kibana"]["password"].ToString());
        
        
        private Capsule<JObject> _config;
        private const string FExistsErr = "Error loading config: configuration file not found or could not be read.";

        public ConfigLoader() {
            const string filepath = "config.json";
            try {
                var rawJson = File.ReadAllText(filepath);
                _config = new Success<JObject>(JObject.Parse(rawJson));
            }
            catch (Exception e) {
                _config = new Failure<JObject>(FExistsErr);
            }

        }
    }
}
