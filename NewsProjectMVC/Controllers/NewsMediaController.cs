using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public async Task<IActionResult> Index(string searchTerm, int? categoryId, string tag)
        {
            // I used the IQueryable to create a query, which will not execute immediately as I can add to it.
            IQueryable<News> query = _context.News.AsQueryable();

            // --- Applying the filters conditionally ---

            // 1. Filter by Search Term (Title)
            // If a search term is provided, add a Where clause to filter by title.
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(newsArticle => newsArticle.Title.ToLower().Contains(searchTerm.ToLower()));
            }

            // 2. Filter by Category
            // If a categoryId is provided, add a Where clause to filter by that category.
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(newsArticle => newsArticle.CategoryId == categoryId.Value);
            }

            // 3. Filter by Tag
            // If a tag is provided, add a Where clause to filter by tags.
            // This checks if the comma-separated 'Tags' string contains the given tag.
            if (!string.IsNullOrEmpty(tag))
            {
                query = query.Where(newsArticle => newsArticle.Tags.ToLower().Contains(tag.ToLower()));
            }

            // --- Prepare data for the view ---

            // Load the list of categories to populate the filter dropdown in the view.
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Title", categoryId);

            // Pass the current filter values back to the view, so the search boxes don't clear after searching.
            ViewBag.CurrentSearchTerm = searchTerm;
            ViewBag.CurrentTag = tag;

            // --- Execute the query and return the result ---
            // Execute final query here with ordering by the CreatedAt (newest first)
            var filteredNews = await query.OrderByDescending(n => n.CreatedAt).ToListAsync();

            return View(filteredNews);
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
                    commentModel.IsApproved = false;

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
