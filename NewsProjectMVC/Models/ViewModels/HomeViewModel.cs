using NewsProjectMVC.Models.Db;

namespace NewsProjectMVC.Models.ViewModels
{
    public class HomeViewModel
    {
        // For single news items like MainNews and TopStory
        public News? MainNews { get; set; }
        public News? TopStory { get; set; }

        // For lists of news items like FeaturesNews and BestNews
        public List<News> FeaturedNews { get; set; } = new List<News>();
        public List<News> BestNews { get; set; } = new List<News>();
       // public List<News> LatestNews { get; set; } = new List<NewsView>();
       // public List<News> MostViewsNews { get; set; } = new List<NewsView>();

        public List<MainPageCategoryViewModel> MainPageCategories { get; set; } = new List<MainPageCategoryViewModel>();


    }

    public class MainPageCategoryViewModel
    {
        public Category? Category { get; set; }
        public List<News> News { get; set; } = new List<News>();
    }
}
