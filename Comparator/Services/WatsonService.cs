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
        /// Performs a natural language understanding analysis using IBM Watson's natural
        /// language understanding service
        /// </summary>
        /// <param name="features">Feature object containing information about the analysis to be performed</param>
        /// <returns>Returns a capsule containing an AnalysisResults object</returns>
        public Capsule<AnalysisResults> PerformNluAnalysis(Features features) {
            var result = _nluService.Analyze(features);

            if (result.StatusCode != StatusCodes.Status200OK) {
                return new Failure<AnalysisResults>($"Status code: {result.StatusCode} Nlu analysis failed", _logger);
            }

            return new Success<AnalysisResults>(result.Result);
        }
    }
}