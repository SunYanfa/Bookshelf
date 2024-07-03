using Bookshelf.Server.DataObject;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Xml.Linq;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bookshelf.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly ILogger<FileUploadController> _logger;
        private readonly BookshelfDBContext _dbContext;

        public FileUploadController(ILogger<FileUploadController> logger, BookshelfDBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
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
                try
                {
                    var books = JsonSerializer.Deserialize<List<BookReader>>(jsonString);
                    if (books != null)
                    {
                        foreach (var book in books)
                        {
                            Thread.Sleep(1000);

                            if (book.name != null && book.author != null)
                            {
                                var exist = await VerifyBookExists(book.name, book.author);
                                if (!exist)
                                {
                                    Book? newBook = await SearchBookInformation(book.name, book.author);

                                    if (newBook != null)
                                    {
                                        await AddBook(newBook);
                                    }
                                    else
                                    {
                                        await AddErrorBook(book.name, book.author, 0);
                                    }

                                }
                            
                            }
                        
                        }
                    }

                }  catch (Exception ex)
                {
                    _logger.LogInformation($"File upload failed.{ex.Message}");
                }


                return Ok(new { filePath = filePath });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the file.");
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task<bool> VerifyBookExists(string novelName, string authorName)
        {
            var books = await _dbContext.Books
                 .Where(b => b.NovelName.Contains(novelName))
                 .Where(b => b.AuthorName.Contains(authorName))
                 .ToListAsync();

            if (books == null || books.Count == 0)
            {
                return false;
            }
            return true;
        }
        
        private async Task AddBook(Book book)
        {
            _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();
        }

        private async Task AddErrorBook (string novelName, string authorName, int errorType)
        {
            ErrorBook errorBook = new()
            {
                NovelName = novelName,
                AuthorName = authorName,
            };
            if (errorType == 0)
            {
                errorBook.ErrorType = errorType;
                errorBook.ErrorDescription = "Not found on this JJWXC";
            } else
            {
                errorBook.ErrorType = errorType;
                errorBook.ErrorDescription = "Other exception";
            }
            _dbContext.ErrorBooks.Add(errorBook);
            await _dbContext.SaveChangesAsync();
        }



        private async Task<Book?> SearchBookInformation(string novelName, string authorName)
        {
            using var client = new HttpClient();
            try
            {
                var url = $"https://android.jjwxc.net/androidapi/search?keyword={novelName}";
                var result = await client.GetFromJsonAsync<SearchJinJiang>(url);

                if (result == null || result.items.Count() == 0)
                {
                    _logger.LogWarning($"No results found for {novelName}");
                    return null;
                }
                var book = result.items[0];
                if (book.authorname == authorName)
                {
                    var bookEntity = new Book
                    {
                        NovelIdJinJiang = book.novelid,
                        NovelName = book.novelname,
                        AuthorName = book.authorname,
                        NovelClass = book.novelClass,
                        NovelTags = book.tags,
                        NovelCover = book.cover,
                        NovelIntro = book.novelintro,
                        NovelIntroShort = book.novelintroshort,
                        NovelSize = book.novelsize,
                        Novelbefavoritedcount = string.Empty,
                        IsSaved = false,                       
                        NovelStep = book.novelstep,
                        NovelDate = DateTimeOffset.FromUnixTimeSeconds(book.novelborndate).DateTime
                    };

                    _logger.LogInformation($"{book.novelname} genera");
                    return bookEntity;
                }

                await AddErrorBook(novelName, authorName, 0);

                return null;
            }
            catch (Exception ex)
            {
                await AddErrorBook(novelName, authorName, 0);
                _logger.LogError(ex, $"An error occurred while searching book information for {novelName} by {authorName}");
                return null;
            }
        }
    }
}
