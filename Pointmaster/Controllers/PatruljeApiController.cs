using Microsoft.AspNetCore.Mvc;
using Pointmaster.Repositories;

namespace Pointmaster.Controllers
{
    [Route("/api/v1/patrulje")]
    public class PatruljeApiController : Controller
    {
        private readonly IPatruljeRepository patruljeRepository;

        public PatruljeApiController(IPatruljeRepository patruljeRepository)
        {
            this.patruljeRepository = patruljeRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var retval = patruljeRepository.GetAll();

            return Ok(retval);
        }

        [HttpPost]
        public IActionResult Index([FromBody] string name)
        {
            patruljeRepository.AddPatrulje(new Patrulje(name));

            return Ok();
        }
    }
}