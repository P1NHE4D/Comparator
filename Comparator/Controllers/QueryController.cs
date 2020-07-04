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
        /// <returns>JSON object containing query results</returns>
        /// <response code="200">Returns a JSON object containing the QueryResult</response>
        /// <response code="400">Invalid query</response>
        [HttpGet]
        [Produces("application/json")]
        [Route("/query")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<QueryResult> SendQuery([FromQuery] string objA, string objB, string aspects) {
            if (string.IsNullOrWhiteSpace(objA)) return BadRequest("Object A is invalid. (Empty or Null)");
            if (string.IsNullOrWhiteSpace(objB)) return BadRequest("Object B is invalid. (Empty or Null)");
            var ip = Request.HttpContext.Connection.RemoteIpAddress;
            var header = Request.Headers;
            var body = Request.Body;
            var path = Request.Path;
            _logger.LogInfo($"IP: {ip} \n PATH: {path} \n HEADER: {header} \n BODY: {body}");
            return _dataAnalyser.AnalyseQuery(objA, objB, aspects?.Split(" "))
                                .Map(r => (ActionResult) Ok(r))
                                .Catch(e => BadRequest(new QueryResult {
                                    Message = e
                                }));
        }
    }
}