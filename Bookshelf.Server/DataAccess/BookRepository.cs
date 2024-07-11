using Bookshelf.Server.DataObject;
using Bookshelf.Server.Helper;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.Server.DataAccess
{
    public class BookRepository
    {
        private readonly BookshelfDBContext _dbContext;

        public BookRepository ( BookshelfDBContext dbContext){
            _dbContext = dbContext;
        }

        public IEnumerable<Book> GetAllBooks()
        {
            return _dbContext.Books.ToArray();
        }

        public async Task<bool> VerifyBookExists(string novelName, string authorName)
        {
            var book = await _dbContext.Books
                 .Where(b => b.NovelName.Contains(novelName))
                 .Where(b => b.AuthorName.Contains(authorName))
                 .ToListAsync();

            return book.Any();
        }

        public async Task AddBook(Book book)
        {
            _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddErrorBook(string novelName, string authorName, int errorType, string? message)
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


        public async Task UpdateBookWithId(Book book)
        {
            _dbContext.Books.Update(book);
            await _dbContext.SaveChangesAsync();
        }
    }
}
