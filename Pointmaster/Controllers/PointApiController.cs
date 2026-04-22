using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pointmaster.Repositories;

namespace Pointmaster.Controllers
{
    [Route("/api/v1/points")]
    [Authorize]
    public class PointApiController : Controller
    {
        private readonly IPointRepository pointRepository;
        private readonly IPatruljeRepository patruljeRepository;
        private readonly IPostRepository postRepository;
        private readonly ITenantAccessService tenantAccessService;

        public PointApiController(
            IPointRepository pointRepository,
            IPatruljeRepository patruljeRepository,
            IPostRepository postRepository,
            ITenantAccessService tenantAccessService
        )
        {
            this.pointRepository = pointRepository;
            this.patruljeRepository = patruljeRepository;
            this.postRepository = postRepository;
            this.tenantAccessService = tenantAccessService;
        }

        private async Task<string> GetAuthorizedTenantId(params string[] allowedRoles)
        {
            var tenantId = Request.Headers["X-Tenant-Id"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                return null;
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            if (allowedRoles.Length == 0)
            {
                return await tenantAccessService.CanAccessTenant(userId, tenantId) ? tenantId : null;
            }

            return await tenantAccessService.IsInRole(userId, tenantId, allowedRoles) ? tenantId : null;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var tenantId = await GetAuthorizedTenantId(TenantRoles.Administrator, TenantRoles.PostUser);
            if (tenantId == null)
            {
                return Forbid();
            }

            var retval = await pointRepository.GetAll(tenantId);

            return Ok(retval);
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] PointDTO data)
        {
            var tenantId = await GetAuthorizedTenantId(TenantRoles.Administrator, TenantRoles.PostUser);
            if (tenantId == null)
            {
                return Forbid();
            }

            var patrulje = await patruljeRepository.GetPatruljeById(data.patrulje, tenantId);
            var post = await postRepository.GetPostById(data.post, tenantId);
            if (patrulje == null || post == null)
            {
                return BadRequest(new { message = "Invalid patrulje or post for selected tenant." });
            }

            await pointRepository.AddPoint(new Point
            {
                Patrulje = patrulje,
                Points = data.point,
                Post = post,
                Turnout = data.turnout,
                TenantId = tenantId
            });

            return Ok(data);
        }

        [HttpDelete]
        public async Task<IActionResult> Index([FromBody] int Id)
        {
            var tenantId = await GetAuthorizedTenantId(TenantRoles.Administrator);
            if (tenantId == null)
            {
                return Forbid();
            }

            await pointRepository.DeletePoint(Id, tenantId);

            return Ok();
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