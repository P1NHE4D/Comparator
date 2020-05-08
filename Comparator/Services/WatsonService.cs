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
            _nluService = new NaturalLanguageUnderstandingService("1.0", authenticator);
            _nluService.SetServiceUrl(
                "");
        }
        
        /// <summary>
        /// Performs a natural language analysis of the text passed as an argument
        /// </summary>
        /// <param name="text">text to be analysed</param>
        /// <param name="features">feature object containing the selected options</param>
        /// <returns>returns a capsule containing AnalysisResults object</returns>
        public Capsule<AnalysisResults> AnalyseText(string text, Features features) {
            var result = _nluService.Analyze(features, text: text);

            if (result.StatusCode != StatusCodes.Status200OK) {
                return new Failure<AnalysisResults>($"Status code: {result.StatusCode} Text analysis failed", _logger);
            }
            return new Success<AnalysisResults>(result.Result);
        }

        /// <summary>
        /// Performs a natural language analysis of the text passed as an argument
        /// </summary>
        /// <param name="url">text to be analysed</param>
        /// <param name="features">feature object containing the selected options</param>
        /// <param name="clean">disables webpage cleansing</param>
        /// <returns>returns a capsule containing AnalysisResults object</returns>
        public Capsule<AnalysisResults> AnalyseUrl(string url, Features features, bool clean = true) {
            var result = _nluService.Analyze(features, url: url, clean: clean);

            if (result.StatusCode != StatusCodes.Status200OK) {
                return new Failure<AnalysisResults>($"Status code: {result.StatusCode} Text analysis failed", _logger);
            }
            return new Success<AnalysisResults>(result.Result);
        }

        /// <summary>
        /// Performs a natural language analysis of the text passed as an argument
        /// </summary>
        /// <param name="html">text to be analysed</param>
        /// <param name="features">feature object containing the selected options</param>
        /// <param name="clean">disables webpage cleansing</param>
        /// <returns>returns a capsule containing AnalysisResults object</returns>
        public Capsule<AnalysisResults> AnalyseHtml(string html, Features features, bool clean = true) {
            var result = _nluService.Analyze(features, html: html, clean: clean);

            if (result.StatusCode != StatusCodes.Status200OK) {
                return new Failure<AnalysisResults>($"Status code: {result.StatusCode} Text analysis failed", _logger);
            }
            return new Success<AnalysisResults>(result.Result);
        }
    }
}