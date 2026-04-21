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
        public IActionResult Index()
        {
            var retval = postRepository.GetAll();

            return Ok(retval);
        }

        [HttpPost]
        public IActionResult Index([FromBody] string name)
        {
            postRepository.AddPost(new Post(name));

            return Ok();
        }
    }
}