using System;
using Comparator.Models;
using Comparator.Utils.Logger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Comparator.Controllers {
    [ApiController]
    [Route("api")]
    public class QueryController : ControllerBase {
        private readonly ILoggerManager _logger;

        private static QueryResult SampleData = new QueryResult {
            ProcessedDataSets = 10734,
            ComputationTime = TimeSpan.FromHours(2.0).TotalSeconds
        };

        public QueryController(ILoggerManager logger) {
            _logger = logger;
        }

        /// <summary>
        /// Analyses the query using the Kibana data set and IBM Watson
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// </remarks>
        /// <param name="query"></param>
        /// <returns>JSON object containing query results</returns>
        /// <response code="200">Returns a JSON object containing the QueryResult</response>
        /// <response code="400">Invalid query</response>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<QueryResult> SendQuery([FromBody] Query query) {
            // TODO: send query to Kibana and process data using IBM watson
            var clientIp = Request.HttpContext.Connection.RemoteIpAddress;
            _logger.LogInfo($"Query received from {clientIp}: {query.Keywords}");
            SampleData.Query = query;
            return Ok(SampleData);
        }
    }
}