using System;
using Comparator.Utils.Configuration;
using Comparator.Utils.Logger;
using Comparator.Utils.Monads;
using IBM.Cloud.SDK.Core.Authentication.Iam;
using IBM.Watson.NaturalLanguageUnderstanding.v1;
using IBM.Watson.NaturalLanguageUnderstanding.v1.Model;
using Microsoft.AspNetCore.Http;

namespace Comparator.Services {
    public class WatsonService : IWatsonService {
        private readonly ILoggerManager _logger;
        private readonly Capsule<NaturalLanguageUnderstandingService> _nluService;

        public WatsonService(ILoggerManager logger, IConfigLoader configLoader) {
            _logger = logger;
            _nluService = from apiKey in configLoader.WatsonApiKey
                          let authenticator = new IamAuthenticator(apiKey)
                          select new NaturalLanguageUnderstandingService("2020-05-01", authenticator);

            _nluService.Access(nlus => configLoader.WatsonUrl.Access(nlus.SetServiceUrl));
        }
        
        /// <summary>
        /// Performs a natural language analysis of the text passed as an argument
        /// </summary>
        /// <param name="text">text to be analysed</param>
        /// <param name="features">feature object containing the selected options</param>
        /// <param name="language">sets the language of the text to be analysed according to the ISO 639-1 standard</param>
        /// <returns>returns a capsule containing AnalysisResults object</returns>
        public Capsule<AnalysisResults> AnalyseText(string text, Features features, string language = "en") {
            try {
                return _nluService.Map(nlus => nlus.Analyze(features, text: text, language: language))
                                  .Bind(result => 
                                            (result.StatusCode != StatusCodes.Status200OK) ? 
                                                Capsule<AnalysisResults>.CreateFailure($"Status code: {result.StatusCode}. Invalid request.", _logger) : 
                                                Capsule<AnalysisResults>.CreateSuccess(result.Result));
            }
            catch (Exception e) {
                _logger.LogError(e.Message);
                return new Failure<AnalysisResults>($"Status code: {StatusCodes.Status500InternalServerError}. Text analysis failed.", _logger);
            }
        }

        /// <summary>
        /// Performs a natural language analysis of the text passed as an argument
        /// </summary>
        /// <param name="url">text to be analysed</param>
        /// <param name="features">feature object containing the selected options</param>
        /// <param name="clean">disables webpage cleansing</param>
        /// <param name="language">sets the language of the webpage to be analysed according to the ISO 639-1 standard</param>
        /// <returns>returns a capsule containing AnalysisResults object</returns>
        public Capsule<AnalysisResults> AnalyseUrl(string url, Features features, bool clean = true, string language = "en") {
            try {                
                return _nluService.Map(nlus => nlus.Analyze(features, url: url, language: language))
                                  .Bind(result => 
                                            (result.StatusCode != StatusCodes.Status200OK) ? 
                                                Capsule<AnalysisResults>.CreateFailure($"Status code: {result.StatusCode}. Invalid request.", _logger) : 
                                                Capsule<AnalysisResults>.CreateSuccess(result.Result));
            }
            catch (Exception e) {
                _logger.LogError(e.Message);
                return new Failure<AnalysisResults>($"Status code: {StatusCodes.Status500InternalServerError}. Text analysis failed.", _logger);
            }

        }

        /// <summary>
        /// Performs a natural language analysis of the text passed as an argument
        /// </summary>
        /// <param name="html">text to be analysed</param>
        /// <param name="features">feature object containing the selected options</param>
        /// <param name="clean">disables webpage cleansing</param>
        /// <param name="language">sets the language of the html to be analysed according to the ISO 639-1 standard</param>
        /// <returns>returns a capsule containing AnalysisResults object</returns>
        public Capsule<AnalysisResults> AnalyseHtml(string html, Features features, bool clean = true, string language = "en") {
            try {
                return _nluService.Map(nlus => nlus.Analyze(features, html: html, language: language))
                                  .Bind(result => 
                                            (result.StatusCode != StatusCodes.Status200OK) ? 
                                                Capsule<AnalysisResults>.CreateFailure($"Status code: {result.StatusCode}. Invalid request.", _logger) : 
                                                Capsule<AnalysisResults>.CreateSuccess(result.Result));
            }
            catch (Exception e) {
                _logger.LogError(e.Message);
                return new Failure<AnalysisResults>($"Status code: {StatusCodes.Status500InternalServerError}. Text analysis failed.", _logger);
            }
        }
    }
}