using Microsoft.AspNetCore.Mvc;
using ComputeLayer.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;

namespace ComputeLayer.Controllers
{
    [ApiController]
    [Route("/query")]
    public class HomeController : Controller
    {

        [HttpPost]
        public async Task<IActionResult> AddJSON([FromBody] Query query)
        {
            try
            {
                // From Task 1 : Convert Kusto query to SQL query
                string sqlQuery = ConvertToSql(query);

                // Make HTTP request to Storage Layer
                using (HttpClient httpClient = new HttpClient())
                {
                    var content = new StringContent(JsonConvert.SerializeObject(new { query = sqlQuery }), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync("https://localhost:7076/api/query", content);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();

                        // Convert JSON response to list of rows
                        //test the data 
                        //get the data and convert it into a list 
                        List<List<string>> rows = JsonConvert.DeserializeObject<List<List<string>>>(responseData);

                        // Return data to user
                        return Ok(rows);
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle errors
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private string ConvertToSql(Query query)
        {
            // TODO: Implement Kusto to SQL query conversion logic
            return "SELECT * FROM <table-name>";
        }


    }
}

