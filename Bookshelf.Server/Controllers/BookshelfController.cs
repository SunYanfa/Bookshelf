using Bookshelf.Server.DataObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Bookshelf.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookshelfController : ControllerBase
    {
        private readonly ILogger<BookshelfController> _logger;
        private readonly BookshelfDBContext _dbContext;

        public BookshelfController(ILogger<BookshelfController> logger, BookshelfDBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet(Name = "GetBookshelf")]
        public IEnumerable<Book> Get()
        {
            return _dbContext.Books.ToArray();
        }
    }
}
