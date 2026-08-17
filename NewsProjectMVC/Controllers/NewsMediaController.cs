using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsProjectMVC.Models.Db;
using NewsProjectMVC.Models.Helpers;
using NewsProjectMVC.Models.ViewModels;

namespace NewsProjectMVC.Controllers
{
    public class NewsMediaController : Controller
    {
        private readonly MyNewsContext _context;

        public NewsMediaController(MyNewsContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet("news/{id}")]
        public async Task<IActionResult> NewsDetails(int id)
        {
            var news = await _context.News.FirstOrDefaultAsync(newsRecord => newsRecord.Id == id);
            if (news == null)
            {
                return NotFound();
            }
            news.ViewCount++;
            _context.News.Update(news);
            await _context.SaveChangesAsync();

            //------------------
            //I will use these later when I create more tables in the SQL Server database
            //var comments = await _context.Comments.Where(x => x.NewsId == id && x.IsApproved).OrderByDescending(x => x.Id).ToListAsync();

            var popularCategories = await _context.PopularCategories.OrderByDescending(x => x.NewsCount).Take(10).ToListAsync();

            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == news.CategoryId);

            
            var relatedNews = await _context.News.Where(x => x.CategoryId == category.Id && x.Id != news.Id).Take(2).ToListAsync();

            //var currentNewsId = news.Id;

            //var popularNews = await _context.PopularNews.OrderByDescending(x => x.CommentCount).Take(5).ToListAsync();

            //------------------
            var result = new NewsDetailsViewModel()
            {
                NewsData = news,
                //Comments = comments,
                Category = category,
                ReadingTimeInMinutes = TextHelpers.CalculateReadingTime(news.LongDescription),
                RelatedNews = relatedNews,
                //PopularCategories = popularCategories,
                //PopularNews = popularNews
            };
            //------------------

            return View(result);
        }


    }
}
