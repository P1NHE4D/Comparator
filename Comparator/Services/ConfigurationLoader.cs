using System;
using System.IO;
using Comparator.Utils.Monads;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace Comparator.Services {
    public class ConfigurationLoader : IConfigurationLoader {
        public Capsule<string> KibanaUsername { get; }
        public Capsule<string> KibanaPassword { get; }
        public Capsule<string> WatsonUsername { get; }
        public Capsule<string> WatsonPassword { get; }
        private const string FExistsErr = "Error loading config: configuration file not found";

        public ConfigurationLoader() {
            const string filepath = "config.json";
            if (!File.Exists(filepath)) {
                this.KibanaUsername = new Failure<string>(FExistsErr);
                this.KibanaPassword = new Failure<string>(FExistsErr);
                this.WatsonUsername = new Failure<string>(FExistsErr);
                this.WatsonPassword = new Failure<string>(FExistsErr);
            }
            else {
                var rawJson = File.ReadAllText(filepath);
                ConfigurationJson jsonObj = JsonSerializer.Deserialize<ConfigurationJson>(rawJson);

                this.KibanaUsername = new Success<string>(jsonObj.KibanaUsername);
                this.KibanaPassword = new Success<string>(jsonObj.KibanaPassword);
                this.WatsonUsername = new Success<string>(jsonObj.WatsonUsername); 
                this.WatsonPassword = new Success<string>(jsonObj.WatsonPassword);
            }
        }
        private class ConfigurationJson {
            public string KibanaUsername { get; set; }
            public string KibanaPassword { get; set; }
            public string WatsonUsername { get; set; }
            public string WatsonPassword { get; set; }
        }
    }
}
