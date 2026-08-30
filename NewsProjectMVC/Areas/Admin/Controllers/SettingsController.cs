
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

    private bool SettingExists(int? id)
    {
        return _context.Settings.Any(e => e.Id == id);
    }
}
