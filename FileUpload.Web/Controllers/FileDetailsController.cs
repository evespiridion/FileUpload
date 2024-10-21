using FileUpload.Domain.Entities;
using FileUpload.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.IO;

namespace FileUpload.Web.Controllers
{
    public class FileDetailsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public FileDetailsController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var files = _db.FileDetail.ToList();
            return View(files);
        }

        public IActionResult Insert()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Insert(UploadFileViewModel obj)
        {
            if (ModelState.IsValid && obj.File != null && obj.File.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await obj.File.CopyToAsync(memoryStream);
                    var fileDet = new FileDetails
                    {
                        FileContent = memoryStream.ToArray(), // Store file as byte array
                        FileName = Path.GetFileName(obj.File.FileName), // Store the file name
                        StartDate = obj.StartDate,
                        EndDate = obj.EndDate
                    };

                    _db.FileDetail.Add(fileDet);
                    await _db.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }
            return View(obj);
        }

        public async Task<IActionResult> GetImage(int id)
        {
            var image = await _db.FileDetail.FindAsync(id); // Find the image by ID
            if (image == null)
            {
                return NotFound();
            }

            // Return the file as a memory stream
            var memoryStream = new MemoryStream(image.FileContent);
            Response.Headers["Content-Disposition"] = $"inline; filename={image.FileName}";
            return File(memoryStream, "application/pdf");
        }

        public async Task<IActionResult> DownloadFile(int id)
        {
            var pdfFile = await _db.FileDetail.FindAsync(id);

            if (pdfFile == null)
            {
                return NotFound();
            }

            var currentDate = DateTime.Now;

            if (currentDate < pdfFile.StartDate || currentDate > pdfFile.EndDate)
            {
                return Forbid("File is not accessible at this time.");
            }

            // Return the file as a memory stream
            var memoryStream = new MemoryStream(pdfFile.FileContent);

            return File(memoryStream, "application/pdf", pdfFile.FileName);
        }


    }
}
