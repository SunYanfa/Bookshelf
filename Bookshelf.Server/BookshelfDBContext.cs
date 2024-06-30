using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.Server
{

    public class BookshelfDBContext : DbContext
    {
        public BookshelfDBContext(DbContextOptions<BookshelfDBContext> options) : base(options)
        {
        }

        public DbSet<YourEntity> YourEntities { get; set; }

        // Define other DbSets for your entities
    }

    public class YourEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Define other properties
    }

}
