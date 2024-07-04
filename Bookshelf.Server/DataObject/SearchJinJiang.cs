namespace Bookshelf.Server.DataObject
{
    public class SearchJinJiang
    {
        public Item[] items { get; set; }
        public int count { get; set; }
        public string words { get; set; }
        public string filterNotLikeAuthorText { get; set; }
        public object[] activity { get; set; }
    }

    public class Item
    {
        public string novelintroshort { get; set; }
        public int novelstep { get; set; }
        public int type_id { get; set; }
        public int authorid { get; set; }
        public double reviewrank { get; set; }
        public double reviewnum { get; set; }
        public string tags { get; set; }
        public string novelname { get; set; }
        public string ebookurl { get; set; }
        public int novelborndate { get; set; }
        public int novelid { get; set; }
        public int novelsize { get; set; }
        public string authorname { get; set; }
        public double reviewscore { get; set; }
        public int age { get; set; }
        public string novelreviewexplain { get; set; }
        public string filmname { get; set; }
        public int filmtype { get; set; }
        public string filmtext { get; set; }
        public string filmlabel { get; set; }
        public string reader_novel_tag { get; set; }
        public string cover { get; set; }
        public int local { get; set; }
        public string localImg { get; set; }
        public string novelClass { get; set; }
        public string novelSizeformat { get; set; }
        public string novelintro { get; set; }
        public string favoriteStatus { get; set; }
    }

}
