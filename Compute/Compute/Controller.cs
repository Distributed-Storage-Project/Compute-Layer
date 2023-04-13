using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
        public async Task<IActionResult> AddJSON([FromBody] Query query)
        {
            try
            {
                // Convert Kusto query to SQL
                query = KustoToSqlConverter.Convert(query);

                // Serialize the Query object to JSON
                string json = JsonConvert.SerializeObject(query);

                // Create HttpContent with the JSON representation of the Query object
                HttpContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                // Send Query object to Storage layer
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
