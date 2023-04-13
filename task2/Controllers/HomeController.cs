using ComputeLayer.Models;
using Microsoft.AspNetCore.Mvc;
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

                        var responseDto = JsonConvert.DeserializeObject<QueryResponseDto>(responseData);

                        if (responseDto.IsSuccess)
                        {
                            // Build response to return to user
                            var data = responseDto.Data;
                            responseDto = new QueryResponseDto
                            {
                                IsSuccess = true,
                                Data = data
                            };
                            Console.WriteLine("Response was successful.");
                            return Ok(responseDto);
                        }
                        else
                        {
                            // Handle query error
                            return StatusCode(StatusCodes.Status500InternalServerError, responseDto.Message);
                        }
                    }
                    else
                    {
                        // Handle HTTP error
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

        //i changed output to query here as requested. 
        private Query ConvertToSql(Query query)
        {


            return return KustoToSqlConverter.Convert(query);;

        }

    }
}

