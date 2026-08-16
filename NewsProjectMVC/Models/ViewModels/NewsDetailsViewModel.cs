using NewsProjectMVC.Models.Db;
namespace NewsProjectMVC.Models.ViewModels
{
    public class NewsDetailsViewModel
    {
        public News? NewsData { get; set; }
        public int ReadingTimeInMinutes { get; set; }

        public List<News>? RelatedNews { get; set; }
    }
}
