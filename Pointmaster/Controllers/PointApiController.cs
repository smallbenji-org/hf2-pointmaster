using Microsoft.AspNetCore.Mvc;
using Pointmaster.Repositories;

namespace Pointmaster.Controllers
{
    [Route("/api/v1/points")]
    public class PointApiController : Controller
    {
        private readonly IPointRepository pointRepository;

        public PointApiController(IPointRepository pointRepository)
        {
            this.pointRepository = pointRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var retval = pointRepository.GetAll();

            return Ok(retval);
        }
    }
}