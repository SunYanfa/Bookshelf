using Bookshelf.Server.DataObject;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Bookshelf.Server.DataAccess;
using System.Text;
using Bookshelf.Server.Helper;

namespace Bookshelf.Server.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly ILogger<FileUploadController> _logger;
        private readonly BookshelfHttpClient _bookshelfHttpClient;
        private readonly BookRepository _bookRepository;

        public FileUploadController(ILogger<FileUploadController> logger, BookshelfHttpClient bookshelfHttpClient, BookRepository bookRepository)
        {
            _logger = logger;
            _bookshelfHttpClient = bookshelfHttpClient;
            _bookRepository = bookRepository;
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

            List<Book> listBook = _bookRepository.GetAllBooks().ToList();
            try
            {
                // Read the file content
                string jsonString;
                using (var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
                {
                    jsonString = await reader.ReadToEndAsync();
                }

                // Deserialize JSON content
                var books = JsonSerializer.Deserialize<List<BookReader>>(jsonString);
                if (books != null)
                {
                    foreach (var book in books)
                    {
                        // Use async/await for delay to avoid blocking threads
                        await Task.Delay(1000);

                        bool exists = listBook.Any(b => b.NovelName == book.name && b.AuthorName == book.author);
                        if (!exists)
                        {
                            try
                            {
                                Book? newBook = await SearchBookInformation(book.name, book.author);
                                if (newBook != null)
                                {
                                    newBook.NovelCover = await SaveImageFromUrlAsBase64Async(newBook.NovelCoverUrl);
                                    await _bookRepository.AddBook(newBook);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Error while processing book '{book.name}' by '{book.author}': {ex.Message}");
                            }
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("No books found in the uploaded file.");
                    return BadRequest("Invalid file content.");
                }

                _logger.LogInformation("File upload completed successfully.");
                return Ok();
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "JSON parsing error occurred while processing the file.");
                return BadRequest("Invalid JSON format.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the file.");
                return StatusCode(500, "Internal server error");
            }
        }


        private async Task<Book?> SearchBookInformation(string novelName, string authorName)
        {
            try
            {
                var result = GetSearchResultsAsync(novelName).Result;

                if (result == null || result.items.Count() == 0)
                {
                    await _bookRepository.AddErrorBook(novelName, authorName, 0, "SearchBookInformation result is null");

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
                        NovelCoverUrl = book.cover,
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
                await _bookRepository.AddErrorBook(novelName, authorName, 0, ex.Message);
                _logger.LogError(ex, $"An error occurred while searching book information for {novelName} by {authorName}");
                return null;
            }
        }

        public async Task<SearchJinJiang> GetSearchResultsAsync(string novelName)
        {
            try
            {
                var baseUrl = "https://android.jjwxc.net/androidapi/search";
                var uriBuilder = new UriBuilder(baseUrl)
                {
                    Query = $"keyword={Uri.EscapeDataString(novelName)}"
                };

                HttpResponseMessage response = await _bookshelfHttpClient.GetAsync(uriBuilder.Uri).ConfigureAwait(false);
                response.EnsureSuccessStatusCode(); // 检查响应状态码

                var result = await response.Content.ReadFromJsonAsync<SearchJinJiang>().ConfigureAwait(false);
                return result;
            }
            catch (HttpRequestException httpRequestException)
            {
                Console.WriteLine($"Request error: {httpRequestException.Message}");
                throw;
            }
            catch (TaskCanceledException taskCanceledException) when (taskCanceledException.CancellationToken == default)
            {
                Console.WriteLine("Request timeout.");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw;
            }
        }
        
        public async Task<string> SaveImageFromUrlAsBase64Async(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                throw new ArgumentException("Invalid image URL.");
            }
            byte[] imageBytes = await _bookshelfHttpClient.GetByteArrayAsync(imageUrl);
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }
    }
}
