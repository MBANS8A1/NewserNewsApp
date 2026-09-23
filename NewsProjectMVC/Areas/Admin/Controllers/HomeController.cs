using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsProjectMVC.Models.Db;
using NewsProjectMVC.Models.ViewModels;
using System.Globalization;

namespace NewsProjectMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly MyNewsContext _context;

        public HomeController(MyNewsContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            // --- 1: Calculate the total counts for the statistics cards ---
            AdminDashboardViewModel result = new AdminDashboardViewModel
            {
                NewsCount = await _context.News.CountAsync(),
                CommentsCount = await _context.Comments.CountAsync(),
                CategoriesCount = await _context.Categories.CountAsync(),
                TagsCount = await _context.Tags.CountAsync()
            };

            // --- 2:- Calculate monthly data for the chart ---
            int currentYear = DateTime.Now.Year;

            // Query 1: Get the count of news published per month for the current year.
            var monthlyNewsCounts = await _context.News
                .Where(newsItem => newsItem.CreatedAt.Year == currentYear) // Filter by the current year on the DB.
                .GroupBy(newsItem  => newsItem.CreatedAt.Month)             // Group records by month number on the DB.
                .Select(g => new { Month = g.Key, Count = g.Count() }) // Select the month and its count.
                .ToDictionaryAsync(item => item.Month, item => item.Count); // Fetch data and convert to a dictionary for fast lookups.

            // Query 2: Get the count of comments created per month for the current year.
            var monthlyCommentCounts = await _context.Comments
                .Where(comment => comment.CreatedAt.Year == currentYear) // Assuming the Comment model also has a CreatedAt property.
                .GroupBy(comment => comment.CreatedAt.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToDictionaryAsync(item => item.Month, item => item.Count);


            // --- 3: Populate the ViewModel with the chart results ---
            // I looped through all 12 months to ensure the chart has a complete X-axis.
            for (int month = 1; month <= 12; month++)
            {
                // Add the month's abbreviated name (e.g., "Jan") to the chart labels.
                result.ChartLabels.Add(CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(month));

                // Populate the news data for the current month.
                // I used TryGetValue for safety, which returns the count if the key exists, or 0 otherwise.
                monthlyNewsCounts.TryGetValue(month, out int newsCount);
                result.ChartDataNews.Add(newsCount);

                // Populate the comment data for the current month.
                // We do the same for comments. If no comments were made in a month, we add 0.
                monthlyCommentCounts.TryGetValue(month, out int commentCount);
                result.ChartDataComments.Add(commentCount);
            }

            // Return the view with the fully populated ViewModel.
            return View(result);
        }
    }
}
