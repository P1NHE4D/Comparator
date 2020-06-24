using System;
using Comparator.Models;
using Comparator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Comparator.Controllers {
    [ApiController]
    [Route("api")]
    public class QueryController : ControllerBase {
        private readonly IDataAnalyser _dataAnalyser;

        public QueryController(IDataAnalyser dataAnalyser) {
            _dataAnalyser = dataAnalyser;
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<QueryResult> SendQuery([FromQuery] string objA, string objB, string aspects) {
            if (string.IsNullOrWhiteSpace(objA)) return BadRequest("Object A is invalid. (Empty or Null)");
            if (string.IsNullOrWhiteSpace(objB)) return BadRequest("Object B is invalid. (Empty or Null)");
            return _dataAnalyser.AnalyseQuery(objA, objB, aspects?.Split(" "))
                                .Map(r => (ActionResult) Ok(r))
                                .Catch(e => BadRequest(new QueryResult {
                                    Message = e
                                }));
        }
    }
}