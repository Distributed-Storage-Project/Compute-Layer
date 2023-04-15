using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;

namespace ComputeLayer.Controllers
{
    [ApiController]
    [Route("api/query")]
    public class HomeController : ControllerBase
    {
        private readonly string _queryControllerUrl = "https://localhost:7076/api/query"; // Update with your QueryController endpoint URL
        private readonly HttpClient _httpClient;

        public HomeController()
        {
            _httpClient = new HttpClient();
        }

        [HttpPost]
        public async Task<IActionResult> AddJSON([FromBody] Query query)
        {
            try
            {
                // Convert Kusto query to SQL
                string sqlQuery = KustoToSqlConverter.Convert(query);

                // Create a JSON payload with the parsed query
                var requestBody = JsonConvert.SerializeObject(new { query = sqlQuery });
                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                // Call the QueryController endpoint and get the response
                var response = await _httpClient.PostAsync(_queryControllerUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Return the response content directly as an IActionResult
                return Content(responseContent, "application/json");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
