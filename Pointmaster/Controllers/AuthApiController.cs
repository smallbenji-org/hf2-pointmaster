using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Pointmaster.Controllers
{
    [ApiController]
    [Route("/api/v1/auth")]
    public class AuthApiController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public AuthApiController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
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
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Ok(new { message = "Logged out." });
        }

        [HttpGet("me")]
        public IActionResult Me()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return Unauthorized();
            }

            return Ok(new { username = User.Identity?.Name });
        }

        public class AuthRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
