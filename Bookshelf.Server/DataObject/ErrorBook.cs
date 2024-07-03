using System.ComponentModel.DataAnnotations;

namespace Bookshelf.Server.DataObject
{
    public class ErrorBook
    {
        [Key]
        public int IdErrorBook {  get; set; }
        [Required]
        public string NovelName {  get; set; }
        [Required]
        public string AuthorName {  get; set; }
        public int ErrorType { get; set; }
        public string ErrorDescription { get; set; }
        public bool IsDelete { get; set; } = false;
        public DateTime AddDate { get; set; } = DateTime.Now;

    }
}
