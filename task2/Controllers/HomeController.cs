using Microsoft.AspNetCore.Mvc;
using ComputeLayer.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace ComputeLayer.Controllers
{
    [ApiController]
    [Route("/query")]
    public class HomeController : Controller
    {

        private readonly QueryController _queryController;

        public HomeController(QueryController queryController)
        {
            _queryController = queryController;
        }

        [HttpPost]
        public async Task<IActionResult> AddJSON([FromBody] Query query, [FromServices] QueryController queryController)
        {
            try
            {
                var data = await _queryController.GetQueryResult(query);

                return Ok(data);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /* Query for task:
         * {
         *  "query": "Logs | where Timestamp >= datetime(2016-10-22 05:00) | where Level == \"e\" | limit 19"
         * }
         * Sends 200 response
         * 
         * Multiple line query:
         * Original query:
         * Logs
         * | where Timestamp >= datetime(2015-08-22 05:00) and Timestamp < datetime(2015-08-22 06:00)
         * | where Level == "e" and Service == "Inferences.UnusualEvents_Main"
         * | project Level, Timestamp, Message
         * | limit 10 
         * 
         * Rewrite query as "Logs| where Timestamp >= datetime(2015-08-22 05:00) and Timestamp < datetime(2015-08-22 06:00)| where Level == \"e\" and Service == \"Inferences.UnusualEvents_Main\"| project Level, Timestamp, Message| limit 10"
         * Then it sends a 200 response
         */
    }
}

