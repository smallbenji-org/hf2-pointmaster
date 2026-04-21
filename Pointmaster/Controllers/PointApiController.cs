using Microsoft.AspNetCore.Mvc;
using Pointmaster.Repositories;

namespace Pointmaster.Controllers
{
    [Route("/api/v1/points")]
    public class PointApiController : Controller
    {
        private readonly IPointRepository pointRepository;
        private readonly IPatruljeRepository patruljeRepository;
        private readonly IPostRepository postRepository;

        public PointApiController(
            IPointRepository pointRepository,
            IPatruljeRepository patruljeRepository,
            IPostRepository postRepository
        )
        {
            this.pointRepository = pointRepository;
            this.patruljeRepository = patruljeRepository;
            this.postRepository = postRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var retval = pointRepository.GetAll();

            return Ok(retval);
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] PointDTO data)
        {
            var patrulje = await patruljeRepository.GetPatruljeById(data.patrulje);
            var post = (await postRepository.GetAll()).FirstOrDefault(x => x.Id == data.post);

            await pointRepository.AddPoint(new Point
            {
                Patrulje = patrulje,
                Points = data.point,
                Post = post,
                Turnout = data.turnout
            });

            return Ok(data);
        }

        public class PointDTO
        {
            public int point { get; set; }
            public int turnout { get; set; }
            public int patrulje { get; set; }
            public int post { get; set; }
        }
    }
}