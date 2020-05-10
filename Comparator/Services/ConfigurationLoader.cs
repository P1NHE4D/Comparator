using System;
using System.IO;
using Comparator.Utils.Monads;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace Comparator.Services {
    public class ConfigurationLoader : IConfigurationLoader {
        // Watson
        public Capsule<string> WatsonUrl => Config.Map(r => r["watson"]["url"].ToString());
        public Capsule<string> WatsonUser => Config.Map(r => r["watson"]["user"].ToString());
        public Capsule<string> WatsonPassword => Config.Map(r => r["watson"]["password"].ToString());
        
        // Kibana
        public Capsule<string> KibanaUrl => Config.Map(r => r["kibana"]["url"].ToString());
        public Capsule<string> KibanaUser => Config.Map(r => r["kibana"]["user"].ToString());
        public Capsule<string> KibanaPassword => Config.Map(r => r["kibana"]["password"].ToString());
        
        
        private Capsule<JObject> Config;
        private const string FExistsErr = "Error loading config: configuration file not found or could not be read.";

        public ConfigurationLoader() {
            const string filepath = "config.json";
            try {
                var rawJson = File.ReadAllText(filepath);
                Config = new Success<JObject>(JObject.Parse(rawJson));
            }
            catch (Exception e) {
                Config = new Failure<JObject>(FExistsErr);
            }

        }
    }
}
