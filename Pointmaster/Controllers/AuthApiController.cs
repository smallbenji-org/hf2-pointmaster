using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Pointmaster.Repositories;
using System.Security.Claims;

namespace Pointmaster.Controllers
{
    [ApiController]
    [Route("/api/v1/auth")]
    public class AuthApiController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ITenantRepository tenantRepository;
        private readonly ITenantAccessService tenantAccessService;

        public AuthApiController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ITenantRepository tenantRepository,
            ITenantAccessService tenantAccessService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tenantRepository = tenantRepository;
            this.tenantAccessService = tenantAccessService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Username and password are required." });
            }

            var user = new IdentityUser
            {
                UserName = request.Username,
                NormalizedUserName = request.Username.ToUpperInvariant(),
                SecurityStamp = Guid.NewGuid().ToString("N"),
                ConcurrencyStamp = Guid.NewGuid().ToString("N")
            };

            var result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            await signInManager.SignInAsync(user, isPersistent: false);
            return Ok(new { message = "User created and logged in." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Username and password are required." });
            }

            var result = await signInManager.PasswordSignInAsync(request.Username, request.Password, false, false);

            if (!result.Succeeded)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            return Ok(new { message = "Logged in." });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Ok(new { message = "Logged out." });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var isSuperUser = await tenantRepository.IsSuperUser(userId);
            var tenants = isSuperUser
                ? await tenantRepository.GetAllTenants()
                : await tenantRepository.GetTenantsForUser(userId);

            var tenantIds = tenants.Select(t => t.Id).ToHashSet();
            var selectedTenant = Request.Headers["X-Tenant-Id"].FirstOrDefault();
            var currentTenant = tenantIds.Contains(selectedTenant)
                ? selectedTenant
                : tenants.FirstOrDefault()?.Id;

            string role = null;
            if (!string.IsNullOrWhiteSpace(currentTenant))
            {
                role = isSuperUser
                    ? TenantRoles.SuperUser
                    : await tenantRepository.GetUserRole(userId, currentTenant);
            }

            return Ok(new
            {
                authenticated = true,
                username = User.Identity?.Name,
                isSuperUser,
                currentTenantId = currentTenant,
                role,
                tenants
            });
        }

        [HttpGet("tenants")]
        [Authorize]
        public async Task<IActionResult> GetMyTenants()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var isSuperUser = await tenantRepository.IsSuperUser(userId);
            var tenants = isSuperUser
                ? await tenantRepository.GetAllTenants()
                : await tenantRepository.GetTenantsForUser(userId);

            return Ok(tenants);
        }

        [HttpPost("tenants")]
        [Authorize]
        public async Task<IActionResult> CreateTenant([FromBody] CreateTenantRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Name))
            {
                return BadRequest(new { message = "Tenant name is required." });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var tenant = await tenantRepository.CreateTenant(request.Name);
            await tenantRepository.AddMember(userId, tenant.Id, TenantRoles.Administrator);

            return Ok(tenant);
        }

        [HttpGet("tenants/{tenantId}/members")]
        [Authorize]
        public async Task<IActionResult> GetTenantMembers(string tenantId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var allowed = await tenantAccessService.IsInRole(userId, tenantId, TenantRoles.Administrator);
            if (!allowed)
            {
                return Forbid();
            }

            var members = await tenantRepository.GetMembers(tenantId);
            return Ok(members);
        }

        [HttpPost("tenants/{tenantId}/members")]
        [Authorize]
        public async Task<IActionResult> AddTenantMember(string tenantId, [FromBody] AddTenantMemberRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var allowed = await tenantAccessService.IsInRole(userId, tenantId, TenantRoles.Administrator);
            if (!allowed)
            {
                return Forbid();
            }

            if (request == null || string.IsNullOrWhiteSpace(request.Username) || !IsValidRole(request.RoleName))
            {
                return BadRequest(new { message = "Username and valid role are required." });
            }

            var user = await userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            await tenantRepository.AddMember(user.Id, tenantId, request.RoleName);

            return Ok(new { message = "User added to tenant." });
        }

        [HttpPut("tenants/{tenantId}/members/{memberUserId}")]
        [Authorize]
        public async Task<IActionResult> UpdateTenantMemberRole(string tenantId, string memberUserId, [FromBody] UpdateTenantMemberRoleRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var allowed = await tenantAccessService.IsInRole(userId, tenantId, TenantRoles.Administrator);
            if (!allowed)
            {
                return Forbid();
            }

            if (request == null || !IsValidRole(request.RoleName))
            {
                return BadRequest(new { message = "Valid role is required." });
            }

            await tenantRepository.UpdateMemberRole(memberUserId, tenantId, request.RoleName);
            return Ok(new { message = "Role updated." });
        }

        [HttpPost("superusers/{username}")]
        [Authorize]
        public async Task<IActionResult> GrantSuperUser(string username)
        {
            var callerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(callerId))
            {
                return Unauthorized();
            }

            if (!await tenantRepository.IsSuperUser(callerId))
            {
                return Forbid();
            }

            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            await tenantRepository.SetSuperUser(user.Id, true);
            return Ok(new { message = "Super user granted." });
        }

        [HttpDelete("superusers/{username}")]
        [Authorize]
        public async Task<IActionResult> RevokeSuperUser(string username)
        {
            var callerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(callerId))
            {
                return Unauthorized();
            }

            if (!await tenantRepository.IsSuperUser(callerId))
            {
                return Forbid();
            }

            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            await tenantRepository.SetSuperUser(user.Id, false);
            return Ok(new { message = "Super user revoked." });
        }

        private static bool IsValidRole(string roleName)
        {
            return string.Equals(roleName, TenantRoles.Administrator, StringComparison.OrdinalIgnoreCase)
                || string.Equals(roleName, TenantRoles.PostUser, StringComparison.OrdinalIgnoreCase)
                || string.Equals(roleName, TenantRoles.SuperUser, StringComparison.OrdinalIgnoreCase);
        }

        public class AuthRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class CreateTenantRequest
        {
            public string Name { get; set; }
        }

        public class AddTenantMemberRequest
        {
            public string Username { get; set; }
            public string RoleName { get; set; }
        }

        public class UpdateTenantMemberRoleRequest
        {
            public string RoleName { get; set; }
        }
    }
}
