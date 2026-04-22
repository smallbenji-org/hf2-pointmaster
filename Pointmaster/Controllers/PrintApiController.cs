using Microsoft.AspNetCore.Mvc;
using QRCoder;
using Microsoft.AspNetCore.Authorization;
using Pointmaster.Repositories;

namespace Pointmaster.Controllers
{
    [Route("api/v1/print")]
    [Authorize]
    public class PrintApiController : Controller
    {
        private readonly ITenantAccessService tenantAccessService;

        public PrintApiController(ITenantAccessService tenantAccessService)
        {
            this.tenantAccessService = tenantAccessService;
        }

        [HttpGet("givPoint/{id}")]
        public async Task<IActionResult> GivPoint(int id)
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

            QRCodeGenerator QrGenerator = new();
            // It's safer to include the protocol (http/https)
            string url = $"{Request.Scheme}://{Request.Host}/point/givpoint/{id}?tenantId={tenantId}";

            QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            byte[] qrCodeBytes = new PngByteQRCode(QrCodeInfo).GetGraphic(20);

            return File(qrCodeBytes, "image/png");
        }
    }
}