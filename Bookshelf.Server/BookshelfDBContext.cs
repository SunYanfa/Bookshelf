using Bookshelf.Server.DataObject;
using Microsoft.EntityFrameworkCore;


namespace Bookshelf.Server
{
    public class BookshelfDBContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public BookshelfDBContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sqlite database
            options.UseSqlite(Configuration.GetConnectionString("WebApiDatabase"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasKey(b => b.NovelId);
        }

        public DbSet<Book> Books { get; set; }
    }
}
