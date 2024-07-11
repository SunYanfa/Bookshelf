using System.ComponentModel.DataAnnotations;

namespace Bookshelf.Server.DataObject
{
    public class Book
    {
        [Key]
        public int NovelId { get; set; }
        public int NovelIdJinJiang { get; set; }
        public string NovelName { get; set; }
        public string AuthorName { get; set; }
        public string NovelClass { get; set; }
        public string NovelTags { get; set; }
        public int NovelSize { get; set; } = 0;
        public string? NovelCover { get; set; }
        public string? NovelCoverUrl { get; set; }
        public string? NovelIntro { get; set; }
        public string? NovelIntroShort { get; set; }
        public string? Novelbefavoritedcount { get; set; }

        public bool IsSaved { get; set; } = false;
        public int NovelStep { get; set; } = 0;
        public DateTime NovelDate { get; set; }
        public bool IsDelete { get; set; } = false;
        public bool IsComplete { get; set; } = true;

        public DateTime AddDate { get; set;} = DateTime.Now;
    }
}
