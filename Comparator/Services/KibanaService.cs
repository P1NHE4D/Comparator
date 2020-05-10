using System;
using System.IO;
using System.Net;
using Comparator.Utils.Monads;

namespace Comparator.Services {
    public class KibanaService : IKibanaService{
        
        public Capsule<string> FetchData(string keywords) {
            try {
                var webRequest = WebRequest.Create("https://www.techrepublic.com/forums/discussions/linux-vs-windows-3/");
                using var response = webRequest.GetResponse();
                using Stream content = response.GetResponseStream();
                using var reader = new StreamReader(content ?? throw new NullReferenceException());
                return new Success<string>(reader.ReadToEnd());
            }
            catch (Exception e) {
                return new Failure<string>("An error occurred");
            }

        }
    }
}