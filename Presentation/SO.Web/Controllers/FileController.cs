using Microsoft.AspNetCore.Mvc;
using SO.Application.Abstractions.Storage;

namespace SO.Web.Controllers
{
    public class FileController : Controller
    {
        private readonly IStorageService _storageService;
        private readonly ILogger<FileController> _logger;

        public FileController(IStorageService storageService, ILogger<FileController> logger)
        {
            _storageService = storageService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFileCollection files)
        {
            try
            {
                if (files == null || files.Count == 0)
                {
                    return Json(new { success = false, message = "Dosya seçilmedi!" });
                }

                _logger.LogInformation($"Dosya yükleme başladı. Dosya sayısı: {files.Count}");
                
                var result = await _storageService.UploadAsync("uploads", files);
                
                _logger.LogInformation($"Dosya yükleme tamamlandı. Yüklenen dosya sayısı: {result.Count}");
                
                return Json(new { 
                    success = true, 
                    message = $"{result.Count} dosya başarıyla yüklendi!",
                    files = result.Select(f => new { fileName = f.fileName, path = f.pathOrContainerName })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dosya yükleme hatası");
                return Json(new { success = false, message = "Dosya yükleme hatası: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetFiles()
        {
            try
            {
                var files = _storageService.GetFiles("uploads");
                return Json(new { success = true, files = files });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dosya listesi alma hatası");
                return Json(new { success = false, message = "Dosya listesi alınamadı: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string fileName)
        {
            try
            {
                await _storageService.DeleteAsync("uploads", fileName);
                return Json(new { success = true, message = "Dosya silindi!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dosya silme hatası");
                return Json(new { success = false, message = "Dosya silinemedi: " + ex.Message });
            }
        }
    }
} 