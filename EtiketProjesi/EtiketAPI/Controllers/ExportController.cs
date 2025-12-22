using EtiketAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EtiketAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private readonly ExportService _exportService;
        public ExportController(ExportService exportService)
        {
            _exportService = exportService;
        }

        [HttpGet("yolo/{paylasimKodu}")]
        public async Task<IActionResult> DownloadYolo(string paylasimKodu)
        {
            var zipData = await _exportService.CreateYoloZip(paylasimKodu);

            if (zipData == null)
                return NotFound("Dataset bulunamadı");

            return File(
                zipData,
                "application/zip",
                $"dataset_{paylasimKodu}.zip"
            );
        }
    }
}