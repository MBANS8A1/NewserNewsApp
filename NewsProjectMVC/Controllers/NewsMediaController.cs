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
            var comments = await _context.Comments.Where(comment => comment.NewsId == id && comment.IsApproved).OrderByDescending(comment => comment.Id).ToListAsync();

            var popularCategories = await _context.PopularCategories.OrderByDescending(x => x.NewsCount).Take(10).ToListAsync();

            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == news.CategoryId);

            
            var relatedNews = await _context.News.Where(x => x.CategoryId == category.Id && x.Id != news.Id).Take(2).ToListAsync();

            var currentNewsId = news.Id;

            var popularNews = await _context.PopularNews.OrderByDescending(x => x.CommentCount).Take(4).ToListAsync();

            //------------------
            var result = new NewsDetailsViewModel()
            {
                NewsData = news,
                Comments = comments,
                Category = category,
                ReadingTimeInMinutes = TextHelpers.CalculateReadingTime(news.LongDescription),
                RelatedNews = relatedNews,
                PopularCategories = popularCategories,
                PopularNews = popularNews
            };
            //------------------

            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitComment(Comment commentModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Set server-side properties securely (these cannot be set on the NewsMedia/NewsDetails View).
                    commentModel.CreatedAt = DateTime.Now;
                    commentModel.IsApproved = true;

                    // Add the new comment to the context and save to the database.
                    _context.Comments.Add(commentModel);
                    await _context.SaveChangesAsync();

                    // Redirect the user back to the news article they were on.
                    // I used the TempData dictionary to add a success message (but it will be cleared after use)
                    TempData["SuccessMessage"] = "Your comment has been submitted and is awaiting approval.";
                    return Redirect("/news/" + commentModel.NewsId + "#comment-form");
                }
                catch
                {
                    // In case of a database error, add an error message and redirect.
                    // I used the TempData dictionary to add an error message (but it will be cleared after use)
                    TempData["ErrorMessage"] = "There was an error submitting your comment. Please try again.";
                    return Redirect("/news/" + commentModel.NewsId + "#comment-form");
                }
            }

            // If ModelState is not valid, it means some required fields were empty.
            // Redirect back to the page to show the validation errors.
            TempData["ErrorMessage"] = "Please fill in all required fields.";
            return Redirect("/news/" + commentModel.NewsId + "#comment-form");
        }
    }
}
