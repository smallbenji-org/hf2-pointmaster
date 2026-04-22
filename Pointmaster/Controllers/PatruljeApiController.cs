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
        public async Task<IActionResult> Index()
        {
            var retval = await patruljeRepository.GetAll();

            return Ok(retval);
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] string name)
        {
            await patruljeRepository.AddPatrulje(new Patrulje(name));

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Index([FromBody] int id)
        {
            await patruljeRepository.DeletePatrulje(id);

            return Ok();
        }
    }
}