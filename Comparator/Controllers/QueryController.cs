using System;
using Comparator.Models;
using Comparator.Services;
using Comparator.Utils.Logger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Comparator.Controllers {
    [ApiController]
    [Route("api")]
    public class QueryController : ControllerBase {
        private readonly ILoggerManager _logger;
        private readonly IDataAnalyser _dataAnalyser;

        public QueryController(ILoggerManager logger, IDataAnalyser dataAnalyser) {
            _logger = logger;
            _dataAnalyser = dataAnalyser;
        }

        /// <summary>
        /// Analyses the query using the Kibana data set and IBM Watson
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// </remarks>
        /// <param name="keywords"></param>
        /// <returns>JSON object containing query results</returns>
        /// <response code="200">Returns a JSON object containing the QueryResult</response>
        /// <response code="400">Invalid query</response>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<QueryResult> SendQuery([FromQuery] string keywords) {
            return _dataAnalyser.AnalyseQuery(keywords)
                                .Map(r => (ActionResult) Ok(r))
                                .Catch(e => {
                                    _logger.LogError(e);
                                    return BadRequest(new QueryResult() {
                                        Message = e
                                    });
                                });
        }

        /// <summary>
        /// Demo query to showcase the data retrieved from watson
        /// </summary>
        /// <returns>JSON object containing query results</returns>
        /// <response code="200">Returns a JSON object containing the query results</response>
        /// <response code="400">Invalid request</response>
        [HttpGet]
        [Route("DemoQuery")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<QueryResult> DemoQuery() {
            return _dataAnalyser.AnalyseQuery("Windows Linux")
                                .Map(r => (ActionResult) Ok(r))
                                .Catch(e => {
                                    _logger.LogError(e);
                                    return BadRequest(new QueryResult() {
                                        Message = e
                                    });
                                });
        }
    }
}