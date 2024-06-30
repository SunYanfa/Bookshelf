namespace Bookshelf.Server.DataObject
{
    public class BookJinJiang
    {
        public string novelId { get; set; }
        public string novelName { get; set; }
        public string authorId { get; set; }
        public string authorName { get; set; }
        public string novelClass { get; set; }
        public string novelTags { get; set; }
        public string novelTagsId { get; set; }
        public string novelCover { get; set; }
        public string originalCover { get; set; }
        public string novelStep { get; set; }
        public string novelIntro { get; set; }
        public string novelIntroShort { get; set; }
        public string isVip { get; set; }
        public string isPackage { get; set; }
        public string novelSize { get; set; }
        public string novelsizeformat { get; set; }
        public string novelChapterCount { get; set; }
        public string renewDate { get; set; }
        public string renewChapterId { get; set; }
        public string renewChapterName { get; set; }
        public string novelScore { get; set; }
        public string islock { get; set; }
        public string novelbefavoritedcount { get; set; }
        public string novelbefavoritedcountformat { get; set; }
        public string type_id { get; set; }
        public string age { get; set; }
        public string maxChapterId { get; set; }
        public string chapterdateNewest { get; set; }
        public string local { get; set; }
        public string localImg { get; set; }
        public string novelStyle { get; set; }
        public string series { get; set; }
        public string protagonist { get; set; }
        public string costar { get; set; }
        public string other { get; set; }
        public string comment_count { get; set; }
        public string nutrition_novel { get; set; }
        public string ranking { get; set; }
        public string novip_clicks { get; set; }
        public string vipChapterid { get; set; }
        public string isSign { get; set; }
        public string ILTC { get; set; }
        public string mainview { get; set; }
        public string codeUrl { get; set; }
        public string novelReviewScore { get; set; }
        public string qgmt { get; set; }
        public List<Character> characters { get; set; }
        public List<object> character_relations { get; set; }
        public string cache_create_date { get; set; }
        public string authorsayrule { get; set; }
        public string copystatus { get; set; }
        public string isVipMonth { get; set; }
        public List<object> yellowcard { get; set; }
        public string Is_short_package { get; set; }

    }

    public class Character
    {
        public string novelid { get; set; }
        public string character_id { get; set; }
        public string character_name { get; set; }
        public string character_masked { get; set; }
        public string character_gender { get; set; }
        public string is_pov { get; set; }
        public string character_type { get; set; }
        public string character_pic { get; set; }
        public string dateline { get; set; }
    }


}
