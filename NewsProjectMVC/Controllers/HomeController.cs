using Microsoft.AspNetCore.Mvc;
using NewsProjectMVC.Models;
using NewsProjectMVC.Models.Db;
using NewsProjectMVC.Models.ViewModels;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace NewsProjectMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyNewsContext _context;

        public HomeController(MyNewsContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            // Step 1: Read the general site settings.
            var settings = await _context.Settings.FirstOrDefaultAsync();
            if (settings == null)
            {
                // If no settings are found, return a view with an empty model.
                return View(new HomeViewModel());
            }

            var viewModel = new HomeViewModel();

            // Step 2: Find and assign the "Main News".
            // This block is self-contained and easy to understand.
            if (settings.MainNews.HasValue)
            {
                viewModel.MainNews = await _context.News.FindAsync(settings.MainNews.Value);
            }

            // Step 3: Find and assign the "Top Story".
            // Same simple logic as Main News.
            if (settings.TopStory.HasValue)
            {
                viewModel.TopStory = await _context.News.FindAsync(settings.TopStory.Value);
            }

            // Step 4: Find and assign the list of "Features News".
            if (!string.IsNullOrEmpty(settings.FeaturedNews))
            {
                // First, convert the comma-separated string of IDs into a list of integers.
                var featuredNewsIds = settings.FeaturedNews.Split(',').Select(int.Parse).ToList();

                // Then, query the database to get all news items with those IDs.
                viewModel.FeaturedNews = await _context.News
                                                 .Where(newsItem => featuredNewsIds.Contains(newsItem.Id))
                                                 .Take(4)
                                                 .ToListAsync();
            }

            // Step 5: Find and assign the list of "Best News".
            if (!string.IsNullOrEmpty(settings.BestNews))
            {
                // Same logic as Featured News.
                var bestNewsIds = settings.BestNews.Split(',').Select(int.Parse).ToList();

                viewModel.BestNews = await _context.News
                                             .Where(newsItem => bestNewsIds.Contains(newsItem.Id))
                                             .ToListAsync();
            }


            if (!string.IsNullOrEmpty(settings.MainPageCategories))
            {
                // Step 1: Get the list of selected category IDs from the settings string.
                var mainPageCategoriesIds = settings.MainPageCategories.Split(',').Select(int.Parse).ToList();

                // Step 2: Fetch all the required Category objects in a single query.
                var selectedCategories = await _context.Categories
                                                 .Where(category => mainPageCategoriesIds.Contains(category.Id))
                                                 .ToListAsync();

                // Step 3: Now, in memory, build the final ViewModel structure.
                foreach (var category in selectedCategories)
                {
                    // For each category, find its corresponding news from the list I fetched.
                    // I am taking the top 5 latest news for each category. I can change this number.
                    var newsForThisCategory = await _context.News
                                                   .Where(newsItem => newsItem.CategoryId == category.Id)
                                                   .OrderByDescending(newsItem => newsItem.CreatedAt)
                                                   .Take(5)
                                                   .ToListAsync();

                    // Create the MainPageCategoryViewModel and add it to the main list.
                    viewModel.MainPageCategories.Add(new MainPageCategoryViewModel
                    {
                        Category = category,
                        News = newsForThisCategory
                    });
                }
            }

            //viewModel.LastNews = await _context.NewsViews
            //                                 .OrderByDescending(x => x.Id)
            //                                 .Take(10)
            //                                 .ToListAsync();


            //viewModel.MostViewsNews = await _context.NewsViews
            //                              .OrderByDescending(x => x.ViewCount)
            //                              .Take(10)
            //                              .ToListAsync();

            // Step 6: Return the fully populated ViewModel to the View.
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
