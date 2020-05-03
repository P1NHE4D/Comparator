using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Comparator.Utils.Monads;
using Newtonsoft.Json;

namespace Comparator.OutgoingConnection
{
    public class HttpRequestSender : IHttpRequestSender
    {
        private static readonly Dictionary<string, HttpClient> Clients = new Dictionary<string, HttpClient>();

        private readonly HttpClient _client;

        public HttpRequestSender(string baseUri)
        {
            if (Clients.TryGetValue(baseUri, out _client)) return;
            
            _client = new HttpClient {BaseAddress = new Uri(baseUri)};
            Clients[baseUri] = _client;
        }

        public async Task<Capsule<string>> GetAsString(string path)
        {
            try
            {
                var response = await _client.GetStringAsync(path);
                return new Success<string>(response);
            }
            catch (Exception e)
            {
                return new Failure<string>(e.Message);
            }
        }
        
        public Task<Capsule<TReturn>> GetAs<TReturn>(string path)
        {
            return from response in GetAsString(path)
                   from value in response
                   select JsonConvert.DeserializeObject<TReturn>(value);
        }
        
        public Task<Capsule<string>> PostAsString(string path, object content)
        {
            try
            {
                var postContent = new StringContent(JsonConvert.SerializeObject(content));
                return from response in _client.PostAsync(path, postContent)
                       from responseContent in response.Content.ReadAsStringAsync()
                       select Capsule<string>.CreateSuccess(responseContent);
            }
            catch (Exception e)
            {
                return new Task<Capsule<string>>(() => Capsule<string>.CreateFailure(e.Message));
            }
        }
        
        public Task<Capsule<TReturn>> PostAs<TReturn>(string path, object content)
        {
            return from responseContentCapsule in PostAsString(path, content)
                   from responseContent in responseContentCapsule
                   select JsonConvert.DeserializeObject<TReturn>(responseContent);
        }
    }
}