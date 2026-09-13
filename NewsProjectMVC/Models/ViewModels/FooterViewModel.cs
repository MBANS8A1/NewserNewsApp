using NewsProjectMVC.Models.Db;
namespace NewsProjectMVC.Models.ViewModels
{
    public class FooterViewModel
    {
        public Setting Settings { get; set; } = new Setting();
        public List<Category> Categories { get; set; } = new List<Category>();

        public List<News> RecentPosts { get; set; } = new List<News>();

        public List<News> Gallery { get; set; } = new List<News>();
    }
}
