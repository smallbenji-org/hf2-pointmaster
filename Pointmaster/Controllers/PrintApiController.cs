using Microsoft.AspNetCore.Mvc;
using QRCoder;

namespace Pointmaster.Controllers
{
    [Route("api/v1/print")]
    public class PrintApiController : Controller
    {
        public PrintApiController()
        {

        }

        [HttpGet("givPoint/{id}")]
        public IActionResult GivPoint(int id)
        {
            QRCodeGenerator QrGenerator = new();
            // It's safer to include the protocol (http/https)
            string url = $"{Request.Scheme}://{Request.Host}/point/givpoint/{id}";

            QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            byte[] qrCodeBytes = new PngByteQRCode(QrCodeInfo).GetGraphic(20);

            return File(qrCodeBytes, "image/png");
        }
    }
}