using Microsoft.AspNetCore.Mvc;

namespace Pointmaster.Controllers
{
    [Route("/api/v1/patrulje")]
    public class PatruljeApiController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var retval = new List<Patrulje>
            {
                new Patrulje
                {
                    Id = 1,
                    Name = "Troppen"
                },
                new Patrulje
                {
                    Id = 2,
                    Name = "Ravnene"
                }
            };

            return Ok(retval);
        }
    }
}