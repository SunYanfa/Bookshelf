using Bookshelf.Server.DataObject;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Data;
using Microsoft.EntityFrameworkCore;

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

                                }

                            }

                        }
                    }

                }
                catch (Exception ex)
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

        private async Task AddErrorBook(string novelName, string authorName, int errorType, string? message)
        {
            ErrorBook errorBook = new()
            {
                NovelName = novelName,
                AuthorName = authorName,
            };
            if (errorType == 0)
            {
                errorBook.ErrorType = errorType;
                errorBook.ErrorDescription = message ?? "Not found on this JJ";
            }
            else
            {
                errorBook.ErrorType = errorType;
                errorBook.ErrorDescription = message ?? "Other exception";
            }
            _dbContext.ErrorBooks.Add(errorBook);
            await _dbContext.SaveChangesAsync();
        }


        private async Task<Book?> SearchBookInformation(string novelName, string authorName)
        {
            try
            {
                var result = GetSearchResultsAsync(novelName).Result;

                if (result == null || result.items.Count() == 0)
                {
                    await AddErrorBook(novelName, authorName, 0, "SearchBookInformation result is null");

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
                return null;
            }
            catch (Exception ex)
            {
                await AddErrorBook(novelName, authorName, 0, ex.Message);
                _logger.LogError(ex, $"An error occurred while searching book information for {novelName} by {authorName}");
                return null;
            }
        }

        private static readonly HttpClient client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30) // 设置超时时间
        };

        public async Task<SearchJinJiang> GetSearchResultsAsync(string novelName)
        {
            try
            {
                var baseUrl = "https://android.jjwxc.net/androidapi/search";
                var uriBuilder = new UriBuilder(baseUrl)
                {
                    Query = $"keyword={Uri.EscapeDataString(novelName)}"
                };

                HttpResponseMessage response = await client.GetAsync(uriBuilder.Uri).ConfigureAwait(false);
                response.EnsureSuccessStatusCode(); // 检查响应状态码

                var result = await response.Content.ReadFromJsonAsync<SearchJinJiang>().ConfigureAwait(false);
                return result;
            }
            catch (HttpRequestException httpRequestException)
            {
                // Handle HTTP request specific exceptions
                Console.WriteLine($"Request error: {httpRequestException.Message}");
                throw;
            }
            catch (TaskCanceledException taskCanceledException) when (taskCanceledException.CancellationToken == default)
            {
                // Handle request timeout
                Console.WriteLine("Request timeout.");
                throw;
            }
            catch (Exception ex)
            {
                // Handle other types of exceptions
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw;
            }
        }

    }
}
