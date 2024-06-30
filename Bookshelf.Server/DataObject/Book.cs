namespace Bookshelf.Server.DataObject
{
    public class Book
    {
        public int NovelId { get; set; }
        public string NovelName { get; set; }
        public string AuthorName { get; set; }
        public string NovelClass { get; set; }
        public string NovelTags { get; set; }
        public int NovelSize { get; set; } = 0;
        public string NovelCover { get; set; }
        public string NovelIntro { get; set; }
        public string NovelIntroShort { get; set; }
        public string? Novelbefavoritedcount { get; set; }

        public bool IsSaved { get; set; } = false;
        public int NovelStep { get; set; } = 0;
        public DateTime NovelDate { get; set; }
    }
}
