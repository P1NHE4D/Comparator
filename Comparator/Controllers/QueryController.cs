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
        private readonly IDataAnalyser _dataAnalyser;
        private readonly ILoggerManager _logger;

        public QueryController(IDataAnalyser dataAnalyser, ILoggerManager logger) {
            _dataAnalyser = dataAnalyser;
            _logger = logger;
        }

        /// <summary>
        /// Analyses the query using the Kibana data set and IBM Watson
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// </remarks>
        /// <param name="objA"></param>
        /// <param name="objB"></param>
        /// <param name="aspects"></param>
        /// <param name="quickSearch"></param>
        /// <returns>JSON object containing query results</returns>
        /// <response code="200">Returns a JSON object containing the QueryResult</response>
        /// <response code="400">Invalid query</response>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<QueryResult> SendQuery([FromQuery] string objA, string objB, string aspects, bool quickSearch = true) {
            _logger.LogInfo($"objA: {objA}, objB: {objB}, aspects: {aspects}, quickSearch: {quickSearch}");
            if (string.IsNullOrWhiteSpace(objA)) return BadRequest("Object A is invalid. (Empty or Null)");
            if (string.IsNullOrWhiteSpace(objB)) return BadRequest("Object B is invalid. (Empty or Null)");
            if (objA.ToLower().Equals(objB.ToLower())) return BadRequest("Object A and Object B are the same");
            var ip = Request.HttpContext.Connection.RemoteIpAddress;
            var path = Request.Path;
            _logger.LogInfo($"IP: {ip} \n PATH: {path} \n");
            return _dataAnalyser.AnalyseQuery(objA, objB, aspects?.Split(" "), quickSearch)
                                .Map(r => (ActionResult) Ok(r))
                                .Catch(e => {
                                    _logger.LogInfo(e.Message);
                                    return BadRequest(new QueryResult {
                                        Message = e.Message
                                    });
                                });
        }
    }
}