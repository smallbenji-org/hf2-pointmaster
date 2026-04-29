using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pointmaster.Repositories;
using System.Security.Claims;

namespace Pointmaster.Controllers
{
    [Route("/api/v1/givpoint")]
    public class GivPointApiController : Controller
    {
        private readonly IPointRepository pointRepository;
        private readonly IPostRepository postRepository;
        private readonly IPatruljeRepository patruljeRepository;
        private readonly ITenantAccessService tenantAccessService;

        public GivPointApiController(
            IPointRepository pointRepository,
            IPostRepository postRepository,
            IPatruljeRepository patruljeRepository,
            ITenantAccessService tenantAccessService)
        {
            this.pointRepository = pointRepository;
            this.postRepository = postRepository;
            this.patruljeRepository = patruljeRepository;
            this.tenantAccessService = tenantAccessService;
        }

        private string GetTenantId()
        {
            return Request.Headers["X-Tenant-Id"].FirstOrDefault();
        }

        private async Task<Patrulje> ResolvePatrulje(int patruljeId)
        {
            var tenantId = GetTenantId();
            if (!string.IsNullOrWhiteSpace(tenantId))
            {
                var scopedPatrulje = await patruljeRepository.GetPatruljeById(patruljeId, tenantId);
                if (scopedPatrulje != null)
                {
                    return scopedPatrulje;
                }
            }

            return await patruljeRepository.GetPatruljeById(patruljeId);
        }

        private async Task<string> GetAuthorizedTenantId(params string[] allowedRoles)
        {
            var tenantId = GetTenantId();
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                return null;
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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

        [HttpGet("{patruljeId:int}")]
        public async Task<IActionResult> Overview(int patruljeId)
        {
            var patrulje = await ResolvePatrulje(patruljeId);
            if (patrulje == null)
            {
                return NotFound(new { message = "Patrulje not found." });
            }

            var tenantId = patrulje.TenantId;

            var posts = await postRepository.GetAll(tenantId);
            var patruljePoints = await pointRepository.GetPointByPatrulje(patruljeId, tenantId);

            var overview = posts.Select(post => new GivPointOverviewItem
            {
                Post = post,
                Point = patruljePoints.FirstOrDefault(point => point.Post?.Id == post.Id)
            }).ToList();

            return Ok(new GivPointOverview
            {
                Patrulje = patrulje,
                Posts = overview
            });
        }

        [HttpGet("{patruljeId:int}/posts/{postId:int}")]
        [Authorize]
        public async Task<IActionResult> Post(int patruljeId, int postId)
        {
            var patrulje = await ResolvePatrulje(patruljeId);
            if (patrulje == null)
            {
                return NotFound(new { message = "Patrulje not found." });
            }

            var tenantId = patrulje.TenantId;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "Authentication is required." });
            }

            var allowed = await tenantAccessService.IsInRole(userId, tenantId, TenantRoles.Administrator, TenantRoles.PostUser);
            if (!allowed)
            {
                return Forbid();
            }

            var post = await postRepository.GetPostById(postId, tenantId);
            if (patrulje == null || post == null)
            {
                return BadRequest(new { message = "Invalid patrulje or post for selected tenant." });
            }

            var point = await pointRepository.GetPointByPatruljeAndPost(patruljeId, postId, tenantId);
            return Ok(new
            {
                patrulje,
                post,
                point
            });
        }

        [HttpPut("{patruljeId:int}")]
        [Authorize]
        public async Task<IActionResult> Save(int patruljeId, [FromBody] GivPointUpdateDTO data)
        {
            var patrulje = await ResolvePatrulje(patruljeId);
            if (patrulje == null)
            {
                return NotFound(new { message = "Patrulje not found." });
            }

            var tenantId = patrulje.TenantId;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "Authentication is required." });
            }

            var allowed = await tenantAccessService.IsInRole(userId, tenantId, TenantRoles.Administrator, TenantRoles.PostUser);
            if (!allowed)
            {
                return Forbid();
            }

            var post = await postRepository.GetPostById(data.post, tenantId);
            if (patrulje == null || post == null)
            {
                return BadRequest(new { message = "Invalid patrulje or post for selected tenant." });
            }

            var saved = await pointRepository.SavePoint(new Point
            {
                Patrulje = patrulje,
                Points = data.point,
                Post = post,
                Turnout = data.turnout,
                TenantId = tenantId
            });

            return Ok(new
            {
                message = "Point saved successfully.",
                point = saved
            });
        }
    }
}