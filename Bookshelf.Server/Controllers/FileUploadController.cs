using Bookshelf.Server.DataObject;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Xml.Linq;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Bookshelf.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly ILogger<FileUploadController> _logger;

        public FileUploadController(ILogger<FileUploadController> logger)
        {
            _logger = logger;
        }

        // POST: FileUpload
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            _logger.LogInformation("File upload started.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, file.FileName);

            try
            {
                // Save the file to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Read the file and print its content to the console
                string jsonString = await System.IO.File.ReadAllTextAsync(filePath);

                // 解析JSON文件并转换为实体对象
                var books = JsonSerializer.Deserialize<List<BookReader>>(jsonString);

                if (books != null)
                {
                    foreach (var book in books)
                    {
                        if (book.name != null && book.author != null)
                        {
                            await SearchBookInformation(book.name, book.author);
                        }
                        
                    }
                }

                return Ok(new { filePath = filePath });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the file.");
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task SearchBookInformation(string novelName, string authorName)
        {
            using var client = new HttpClient();
            try
            {
                var url = $"https://android.jjwxc.net/androidapi/search?keyword={novelName}";
                var result = await client.GetFromJsonAsync<SearchJinJiang>(url);

                if (result == null || result.items.Count() == 0)
                {
                    _logger.LogWarning($"No results found for {novelName}");
                    return;
                }

                var book = result.items[0];
                if (book.authorname == authorName)
                {
                    var xml = new XElement("Book",
                        new XElement("NovelName", book.novelname),
                        new XElement("AuthorName", book.authorname),
                        new XElement("NovelClass", book.novelClass),
                        new XElement("NovelTags", book.tags),
                        new XElement("NovelCover", book.cover),
                        new XElement("NovelIntro", book.novelintro),
                        new XElement("NovelIntroShort", book.novelintroshort),
                        new XElement("Novelbefavoritedcount", string.Empty),
                        new XElement("IsSaved", false),
                        new XElement("NovelStep", book.novelstep),
                        new XElement("NovelYear", DateTimeOffset.FromUnixTimeSeconds(book.novelborndate).DateTime)
                    );

                    var datosFolder = Path.Combine(Directory.GetCurrentDirectory(), "Datos");

                    if (!Directory.Exists(datosFolder))
                    {
                        Directory.CreateDirectory(datosFolder);
                    }

                    var xmlFilePath = Path.Combine(datosFolder, "book.xml");
                    xml.Save(xmlFilePath);
                    _logger.LogInformation($"Book information saved to {xmlFilePath}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while searching book information for {novelName} by {authorName}");
            }
        }
    }
}
