using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // create Http client
            HttpClient httpClient = new HttpClient();


            // create Http POST data
            string postData = "{\"queryField\":\"test query\"}";
            HttpContent content = new StringContent(postData, System.Text.Encoding.UTF8, "application/json");


            // create the Http request
            string url = "https://cdemo.free.beeceptor.com";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = content;

            try
            {
                // send the http request and get the response
                HttpResponseMessage response = await httpClient.SendAsync(request);
                //check status code
                //int statusCode = (int)response.StatusCode;
                //Console.WriteLine($"HTTP status code: {statusCode}");



                // read the response as a string
                string responseContent = await response.Content.ReadAsStringAsync();

                // print response to the console
                Console.WriteLine(responseContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" error: {ex.Message}");
            }


            Console.ReadLine();
        }
    }
}