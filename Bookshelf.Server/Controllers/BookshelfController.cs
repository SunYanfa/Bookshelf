using Bookshelf.Server.DataObject;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Bookshelf.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookshelfController : ControllerBase
    {


        private readonly ILogger<BookshelfController> _logger;

        public BookshelfController(ILogger<BookshelfController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetBookshelf")]
        public IEnumerable<Book> Get()
        {


            return Enumerable.Range(1, 25).Select(index => new Book
            {
                NovelId = index,
                NovelName = "刀子精" + index,
                AuthorName = "三千世",
                NovelClass = "衍生-无CP-幻想未来-东方衍生-男主",
                NovelTags = "综漫,少年漫,轻松",
                NovelCover = "https://i9-static.jjwxc.net/novelimage.php?novelid=3430986&coverid=16&ver=a907e272c122b78ed73efa3dc5232d09",
                NovelIntro = "不管是成为大名手中的刀，还是这些维护历史的刀，反正都是刀咯？",
                NovelIntroShort = "宇智波二当家当审神者的故事。",
                Novelbefavoritedcount = "26222",
                IsSaved = true,
                NovelStep = 2,
                NovelDate = DateTime.Parse("2024-06-29 17:00:00")
            })
            .ToArray();
        }
    }
}
