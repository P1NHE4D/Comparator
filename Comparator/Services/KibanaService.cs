using System;
using System.IO;
using System.Net;
using Comparator.Models;
using Comparator.Utils.Monads;

namespace Comparator.Services {
    public class KibanaService : IKibanaService{
        
        public Capsule<KibanaDataSet> FetchData(string keywords) {
            try {
                var webRequest = WebRequest.Create("https://www.techrepublic.com/forums/discussions/linux-vs-windows-3/");
                using var response = webRequest.GetResponse();
                using var content = response.GetResponseStream();
                using var reader = new StreamReader(content ?? throw new NullReferenceException());
                return new Success<KibanaDataSet>(new KibanaDataSet() {
                    Data = reader.ReadToEnd(),
                    Count = 5400
                });
            }
            catch (Exception e) {
                return new Failure<KibanaDataSet>("An error occurred");
            }

        }
    }
}