using Microsoft.AspNetCore.Mvc;
using Pointmaster.Repositories;

namespace Pointmaster.Controllers
{
    [Route("api/v1/stats")]
    public class StatApiController : Controller
    {
        private readonly IPointRepository pointRepository;

        public StatApiController(IPointRepository pointRepository)
        {
            this.pointRepository = pointRepository;
        }

        [HttpGet("points")]
        public async Task<IActionResult> Points()
        {
            var data = await pointRepository.GetAll();

            var retval = data.GroupBy(x => x.Patrulje.Id).Select(g => new
            {
                PatruljeName = g.First().Patrulje.Name,
                TotalPoints = g.Sum(p => p.Points),
                TotalTurnout = g.Sum(p => p.Turnout),
                CombinedTotal = g.Sum(p => p.Points + p.Turnout)
            });

            return Ok(retval);
        }
    }

    public class statViewModel
    {
        public string Name { get; set; }
    }
}