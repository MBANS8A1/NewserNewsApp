
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsProjectMVC.Models.Db;

public class NavbarViewComponent : ViewComponent
{
    private readonly MyNewsContext _context;

    public NavbarViewComponent(MyNewsContext context)
    {
        _context = context;
    }

    // GET: MENUS
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var menuItems = await _context.Menus.ToListAsync();
        //I only have one record for the dbo.Settings table in SQL Server
        var Settings = await _context.Settings.FirstOrDefaultAsync();
        var latestTrendingNews = await _context.News.Where(x => x.Status == "Publish").OrderByDescending(x => x.CreatedAt).Take(5).ToListAsync();
        var tuple_result = Tuple.Create(menuItems,Settings,latestTrendingNews);
        return View(tuple_result);
    }

}
