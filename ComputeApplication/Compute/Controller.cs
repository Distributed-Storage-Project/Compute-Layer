﻿using System;
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
using System.Linq;
using BCrypt.Net;

namespace ComputeLayer.Controllers
{
    [ApiController]
    [Route("query")]
    public class HomeController : ControllerBase
    {
        private readonly string _queryControllerUrl = ""; // Update with your QueryController endpoint URL
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

            var user = UserData.Users.FirstOrDefault(u => u.Username == loginRequest.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
            {
                return Unauthorized("Invalid credentials");
            }

            // Check if the user's role is "SystemAdmin"
            if (user.Role != "SystemAdmin")
            {
                return Unauthorized("User does not have SystemAdmin role");
            }

            var jwtSettings = _jwtSettings.Value;

            // Create a list of claims for the JWT token
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, loginRequest.Username),
            new Claim(ClaimTypes.Role, user.Role) // Assign the role from the user object
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
