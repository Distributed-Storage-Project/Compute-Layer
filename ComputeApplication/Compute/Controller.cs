using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace ComputeLayer.Controllers
{
    [ApiController]
    [Route("query")]
    public class HomeController : ControllerBase
    {
        private readonly string _queryControllerUrl = "https://localhost:7076/api/query"; // Update with your QueryController endpoint URL
        private readonly HttpClient _httpClient;
        private readonly IOptions<JwtSettings> _jwtSettings;

        

        public HomeController(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings;

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
        [Authorize(Roles = "SystemAdmin")]
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
        [Authorize(Roles = "SystemAdmin")]
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
        [Authorize(Roles = "SystemAdmin")]
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
        [Authorize(Roles = "SystemAdmin")]
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

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            var users = new List<User>
            {
                new User { Username = "admin", Password = "myPassword123", Role = "SystemAdmin" },
                new User { Username = "user", Password = "myPassword123", Role = "User" }
            };

            var user = users.FirstOrDefault(u => u.Username == loginRequest.Username && u.Password == loginRequest.Password);

            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }
            // Validate the user's credentials.
            // This example assumes the user is valid and has the "SystemAdmin" role.
            // Replace this with your own logic for user validation and role assignment.
            if (loginRequest.Username != "admin" || loginRequest.Password != "myPassword123")
            {
                return Unauthorized("Invalid credentials");
            }


            var jwtSettings = _jwtSettings.Value;

            // Create a list of claims for the JWT token
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, loginRequest.Username),
        new Claim(ClaimTypes.Role, "SystemAdmin") // Assign the "SystemAdmin" role to the user
    };

            // Create the security key and signing credentials
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create the JWT token
            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtSettings.TokenExpirationInMinutes),
                signingCredentials: signingCredentials
                    );

            // Serialize the JWT token into a string
            var tokenHandler = new JwtSecurityTokenHandler();
            var serializedToken = tokenHandler.WriteToken(token);

            // Return the serialized JWT token
            return Ok(new { token = serializedToken });
        }
    }
}
