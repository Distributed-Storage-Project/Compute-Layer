using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;

namespace ComputeLayer.Controllers
{
    [ApiController]
    [Route("query")]
    public class HomeController : ControllerBase
    {
        private readonly string _queryControllerUrl = "https://localhost:7076/api/query"; // Update with your QueryController endpoint URL
        private readonly HttpClient _httpClient;

        public HomeController()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            _httpClient = new HttpClient(clientHandler);
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

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] Query query)
        {
            try
            {
                // Convert Kusto query to SQL
                string sqlQuery = KustoToSqlConverter_Delete.Convert(query);

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

        [HttpPost("insert")]
        public async Task<IActionResult> Insert([FromBody] Query query)
        {
            try
            {
                // Convert Kusto query to SQL
                string sqlQuery = KustoToSqlConverter_Insert.Convert(query);

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

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Query query)
        {
            try
            {
                // Convert Kusto query to SQL
                string sqlQuery = KustoToSqlConverter_Create.Convert(query);

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

        [HttpPost("drop")]
        public async Task<IActionResult> Drop([FromBody] Query query)
        {
            try
            {
                // Convert Kusto query to SQL
                string sqlQuery = KustoToSqlConverter_Drop.Convert(query);

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
