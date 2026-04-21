using Microsoft.AspNetCore.Mvc;
using Pointmaster.Repositories;

namespace Pointmaster.Controllers
{
    [Route("/api/v1/post")]
    public class PostApiController : Controller
    {
        private readonly IPostRepository postRepository;

        public PostApiController(IPostRepository postRepository)
        {
            this.postRepository = postRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var retval = await postRepository.GetAll();

            return Ok(retval);
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] string name)
        {
            await postRepository.AddPost(new Post(name));

            return Ok();
        }
    }
}