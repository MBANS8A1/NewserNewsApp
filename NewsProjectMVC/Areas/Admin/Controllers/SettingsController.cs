
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsProjectMVC.Models.Db;

[Area("Admin")]
public class SettingsController : Controller
{
    private readonly MyNewsContext _context;

    public SettingsController(MyNewsContext context)
    {
        _context = context;
    }

    // GET: SETTINGS/Edit/5
    public async Task<IActionResult> Edit()
    {
        var setting = await _context.Settings.FirstOrDefaultAsync();
        if (setting == null)
        {
            return NotFound();
        }
        // --- Load details for single-select fields ---

        // Load details for MainNews
        if (setting.MainNews.HasValue && setting.MainNews.Value > 0)
        {
            ViewBag.MainNewsDetails = await _context.News.FindAsync(setting.MainNews.Value);
        }

        // Load details for TopStory
        if (setting.TopStory.HasValue && setting.TopStory.Value > 0)
        {
            ViewBag.TopStoryDetails = await _context.News.FindAsync(setting.TopStory.Value);
        }

        // --- Load details for multi-select fields ---

        // Load details for FeaturesNews
        if (!string.IsNullOrEmpty(setting.FeaturedNews))
        {
            var featuredNewsIds = setting.FeaturedNews.Split(',').Select(int.Parse).ToList();
            ViewBag.FeaturedNewsDetails = await _context.News
                                                  .Where(news => featuredNewsIds.Contains(news.Id))
                                                  .ToListAsync();
        }

        // Load details for BestNews
        if (!string.IsNullOrEmpty(setting.BestNews))
        {
            var bestNewsIds = setting.BestNews.Split(',').Select(int.Parse).ToList();
            ViewBag.BestNewsDetails = await _context.News
                                              .Where(news => bestNewsIds.Contains(news.Id))
                                              .ToListAsync();
        }

        if (!string.IsNullOrEmpty(setting.MainPageCategories))
        {
            var categoryIds = setting.MainPageCategories.Split(',').Select(int.Parse).ToList();
            ViewBag.MainPageCategoriesDetails = await _context.Categories
                                                      .Where(category => categoryIds.Contains(category.Id))
                                                      .ToListAsync();
        }

        return View(setting);
    }

    // POST: SETTINGS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, Setting setting)
    {

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(setting);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SettingExists(setting.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Edit");
        }
        return View(setting);
    }

    [HttpGet]
    public async Task<IActionResult> SearchNews(string q) // 'q' is the default parameter name for the search term (referring to ajax)
    {
        // If the search term is empty, return no results as an anonymous type
        if (string.IsNullOrEmpty(q))
        {
            return Json(new { results = new List<object>() }); //results is returned as Json with an empty List on results
        }

        // Search the database for news titles that contain the search term.
        // The query is case-insensitive (I used lower case for filtering in the Where() method.
        var newsQuery = _context.News
                              .Where(newsItem => newsItem.Title.ToLower().Contains(q.ToLower()));

        // Project the results into the format required by Select2.
        // Limit the results to the top 10 for performance but I can change it if needed.
        var results = await newsQuery
                            .Select(newsItem => new { id = newsItem.Id, text = newsItem.Title })
                            .Take(10)
                            .ToListAsync();

        // Return the results in the { results: [...] } structure that Select2 expects.
        return Json(new { results });
    }

    private bool SettingExists(int? id)
    {
        return _context.Settings.Any(e => e.Id == id);
    }
}
