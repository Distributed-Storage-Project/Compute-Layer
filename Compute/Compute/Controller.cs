using System;
using System.Net.Http;
using System.Threading.Tasks;
using Kusto.Data.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Http;

namespace ComputeLayer.Controllers
{
    [ApiController]
    [Route("/query")]
    public class HomeController : ControllerBase
    {
        private readonly HttpClient _storageClient;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _storageClient = httpClientFactory.CreateClient("StorageAPI");
        }

        [HttpPost]
        public async Task<IActionResult> QueryAsync([FromBody] String query)
        {
            try
            {
                // Convert Kusto query to SQL
                string sql = KustoToSqlConverter.Convert(query);

                // Create HttpContent with the SQL query as plain text
                HttpContent content = new StringContent(sql, System.Text.Encoding.UTF8, "text/plain");

                // Send SQL query to Storage layer
                var response = await _storageClient.PostAsync("/api/query", content);

                // Check response status code and return result or error message
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return Ok(result);
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, error);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

    }
}
