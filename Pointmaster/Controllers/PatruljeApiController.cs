using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pointmaster.Repositories;

namespace Pointmaster.Controllers
{
    [Route("/api/v1/patrulje")]
    [Authorize]
    public class PatruljeApiController : Controller
    {
        private readonly IPatruljeRepository patruljeRepository;
        private readonly ITenantAccessService tenantAccessService;

        public PatruljeApiController(IPatruljeRepository patruljeRepository, ITenantAccessService tenantAccessService)
        {
            this.patruljeRepository = patruljeRepository;
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

            var retval = await patruljeRepository.GetAll(tenantId);

            return Ok(retval);
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] string name)
        {
            var tenantId = await GetAuthorizedTenantId(TenantRoles.Administrator);
            if (tenantId == null)
            {
                return Forbid();
            }

            await patruljeRepository.AddPatrulje(new Patrulje(name, tenantId));

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Index([FromBody] int id)
        {
            var tenantId = await GetAuthorizedTenantId(TenantRoles.Administrator);
            if (tenantId == null)
            {
                return Forbid();
            }

            await patruljeRepository.DeletePatrulje(id, tenantId);

            return Ok();
        }
    }
}