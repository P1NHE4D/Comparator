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
        public Capsule<AnalysisResults> AnalyseText(string text, Features features, string language = "en") =>
            _nluService.Map(nlus => nlus.Analyze(features, text: text, language: language))
                       .Bind(result =>
                                 (result.StatusCode != StatusCodes.Status200OK)
                                     ? Capsule<AnalysisResults>.CreateFailure(
                                         $"Status code: {result.StatusCode}. Invalid request.", _logger)
                                     : Capsule<AnalysisResults>.CreateSuccess(result.Result))
                       .MapFailure(e => new Exception(
                                       $"Status code: {StatusCodes.Status500InternalServerError}. Text analysis failed. ({e.Message})"));
    }
}