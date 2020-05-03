using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Comparator.Utils.Monads;
using Newtonsoft.Json;

namespace Comparator.Services
{
    /// <summary>
    /// Service ir outgoing connections 
    /// </summary>
    [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
    public class HttpRequestSender : IHttpRequestSender
    {
        /// <summary>
        /// Dictionary for storing HttpClients
        /// </summary>
        private static readonly Dictionary<string, HttpClient> Clients = new Dictionary<string, HttpClient>();

        /// <summary>
        /// Constructor
        /// </summary>
        public HttpRequestSender()
        {
        }

        /// <summary>
        /// Used to get an instance of HttpClient
        /// </summary>
        /// <param name="baseUri">baseUri</param>
        /// <returns>instance of HttpClient</returns>
        private HttpClient GetHttpClient(string baseUri)
        {
            if (Clients.TryGetValue(baseUri, out var client)) return client;
            
            client = new HttpClient {BaseAddress = new Uri(baseUri)};
            Clients[baseUri] = client;
            return client;
        }

        /// <summary>
        /// Sends a GET request and returns the response as string
        /// </summary>
        /// <param name="baseUri">the essential part of the uri</param>
        /// <param name="path">the other part</param>
        /// <returns>result</returns>
        public async Task<Capsule<string>> GetAsString(string baseUri, string path)
        {
            try
            {
                return await GetHttpClient(baseUri)
                    .GetAsync(path)
                    .Bind(async response =>
                {
                    if (response.StatusCode == HttpStatusCode.Accepted)
                        return Capsule<string>.CreateSuccess(await response.Content.ReadAsStringAsync());
                    return Capsule<string>.CreateFailure($"Request failed! (StatusCode: {response.StatusCode}, URI: {baseUri + path})");
                });
            }
            catch (Exception e)
            {
                return new Failure<string>(e.Message);
            }
        }
        
        /// <summary>
        /// Sends a GET request and returns the response as an object
        /// </summary>
        /// <param name="baseUri">the essential part of the uri</param>
        /// <param name="path">the other part</param>
        /// <returns>result</returns>
        public Task<Capsule<TReturn>> GetAs<TReturn>(string baseUri, string path)
        {
            return from response in GetAsString(baseUri, path)
                   from value in response
                   select JsonConvert.DeserializeObject<TReturn>(value);
        }
        
        /// <summary>
        /// Sends a POST request and returns the response as a string
        /// </summary>
        /// <param name="baseUri">the essential part of the uri</param>
        /// <param name="path">the other part</param>
        /// <returns>result</returns>
        public async Task<Capsule<string>> PostAsString(string baseUri, string path, object content)
        {
            try
            {
                var postContent = new StringContent(JsonConvert.SerializeObject(content));    
                return await GetHttpClient(baseUri)
                    .PostAsync(path, postContent)
                    .Bind(async response =>
                {
                    if (response.StatusCode == HttpStatusCode.Accepted)
                        return Capsule<string>.CreateSuccess(await response.Content.ReadAsStringAsync());
                    return Capsule<string>.CreateFailure($"Request failed! (StatusCode: {response.StatusCode}, URI: {baseUri + path})");
                });
            }
            catch (Exception e)
            {
                return Capsule<string>.CreateFailure(e.Message);
            }
        }
        
        /// <summary>
        /// Sends a POST request and returns the response as an object
        /// </summary>
        /// <param name="baseUri">the essential part of the uri</param>
        /// <param name="path">the other part</param>
        /// <returns>result</returns>
        public Task<Capsule<TReturn>> PostAs<TReturn>(string baseUri, string path, object content)
        {
            return from responseContentCapsule in PostAsString(baseUri, path, content)
                   from responseContent in responseContentCapsule
                   select JsonConvert.DeserializeObject<TReturn>(responseContent);
        }
    }
}