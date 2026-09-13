
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsProjectMVC.Models.Db;
using NewsProjectMVC.Models.ViewModels;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;

public class FooterViewComponent : ViewComponent
{
    private readonly MyNewsContext _context;

    public FooterViewComponent(MyNewsContext context)
    {
        _context = context;
    }

    // GET: MENUS
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var footer_result = new FooterViewModel();
        footer_result.Settings = await _context.Settings.FirstAsync();

        // -----
        //----- Section to load data for the "Recent Posts" column of the footer area -----

        // 1. Get the list of selected category IDs from the settings string.
        var mainPageCategoriesIds = footer_result.Settings.MainPageCategories?.Split(',').Select(int.Parse).ToList();

        // 2. Fetch all the required Category objects in a single query.
        footer_result.Categories = await _context.Categories
                                         .Where(category => mainPageCategoriesIds.Contains(category.Id))
                                         .ToListAsync();

        // 3. Retrieve the published News posts ordered by the CreatedAt field/column and limit the number returned
        footer_result.RecentPosts = await _context.News
            .Where(newsItem => newsItem.Status == "Publish")
            .OrderByDescending(newsItem => newsItem.CreatedAt)
            .Take(2)
            .ToListAsync();

        // 4. Retrieve the images for the Gallery column from the published News posts by the Guid and limit the number returned
        footer_result.Gallery = await _context.News
                           .Where(newsItem => newsItem.Status == "Publish")
                           .OrderBy(newsItem => Guid.NewGuid())
                           .Take(6)
                           .ToListAsync();
        // -----
        return View(footer_result);
    }

}
