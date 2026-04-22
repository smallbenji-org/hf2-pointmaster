using Microsoft.AspNetCore.Mvc;
using Pointmaster.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace Pointmaster.Controllers
{
    [Route("api/v1/stats")]
    [Authorize]
    public class StatApiController : Controller
    {
        private readonly IPointRepository pointRepository;
        private readonly ITenantAccessService tenantAccessService;

        public StatApiController(IPointRepository pointRepository, ITenantAccessService tenantAccessService)
        {
            this.pointRepository = pointRepository;
            this.tenantAccessService = tenantAccessService;
        }

        [HttpGet("points")]
        public async Task<IActionResult> Points()
        {
            var tenantId = Request.Headers["X-Tenant-Id"].FirstOrDefault();
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(tenantId) || string.IsNullOrWhiteSpace(userId))
            {
                return Forbid();
            }

            var canAccess = await tenantAccessService.CanAccessTenant(userId, tenantId);
            if (!canAccess)
            {
                return Forbid();
            }

            var data = await pointRepository.GetAll(tenantId);

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