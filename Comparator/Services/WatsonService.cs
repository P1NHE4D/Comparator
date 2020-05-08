using System;
using Comparator.Utils.Logger;
using Comparator.Utils.Monads;
using IBM.Cloud.SDK.Core.Authentication.Iam;
using IBM.Watson.NaturalLanguageUnderstanding.v1;
using IBM.Watson.NaturalLanguageUnderstanding.v1.Model;
using Microsoft.AspNetCore.Http;

namespace Comparator.Services {
    public class WatsonService : IWatsonService {
        private readonly ILoggerManager _logger;
        private readonly NaturalLanguageUnderstandingService _nluService;

        public WatsonService(ILoggerManager logger) {
            //TODO: fetch apikey and url from config file
            _logger = logger;
            var authenticator = new IamAuthenticator(apikey: "");
            _nluService = new NaturalLanguageUnderstandingService("2020-05-01", authenticator);
            _nluService.SetServiceUrl(
                "");
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
                var result = _nluService.Analyze(features, text: text, language: language);
                if (result.StatusCode != StatusCodes.Status200OK) {
                    return new Failure<AnalysisResults>($"Status code: {result.StatusCode}. Invalid request.", _logger);
                }
                return new Success<AnalysisResults>(result.Result);
            }
            catch (Exception e) {
                _logger.LogError(e.Message);
                return new Failure<AnalysisResults>($"Status code: {StatusCodes.Status500InternalServerError}. Text analysis failed.");
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
                var result = _nluService.Analyze(features, url: url, clean: clean, language: language);

                if (result.StatusCode != StatusCodes.Status200OK) {
                    return new Failure<AnalysisResults>($"Status code: {result.StatusCode}. Invalid request.", _logger);
                }
                return new Success<AnalysisResults>(result.Result);
            }
            catch (Exception e) {
                _logger.LogError(e.Message);
                return new Failure<AnalysisResults>($"Status code: {StatusCodes.Status500InternalServerError}. Text analysis failed.");
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
                var result = _nluService.Analyze(features, html: html, clean: clean, language: language);

                if (result.StatusCode != StatusCodes.Status200OK) {
                    return new Failure<AnalysisResults>($"Status code: {result.StatusCode} Text analysis failed",
                                                        _logger);
                }

                return new Success<AnalysisResults>(result.Result);
            }
            catch (Exception e) {
                _logger.LogError(e.Message);
                return new Failure<AnalysisResults>($"Status code: {StatusCodes.Status500InternalServerError}. Text analysis failed.");
            }
        }
    }
}