
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsProjectMVC.Models.Db;
using NewsProjectMVC.Models.ViewModels;

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
        footer_result.Settings = await _context.Settings.FirstOrDefaultAsync();
        return View(footer_result);
    }

}
