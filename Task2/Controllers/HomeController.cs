using Microsoft.AspNetCore.Mvc;
using ComputeLayer.Models;
namespace ComputeLayer.Controllers
{
    [ApiController]
    [Route("/query")]
    public class HomeController : Controller
    {
        [HttpPost]
        public IActionResult addJSON([FromBody] Query query)
        {
            return Ok();
        }
    }
}
