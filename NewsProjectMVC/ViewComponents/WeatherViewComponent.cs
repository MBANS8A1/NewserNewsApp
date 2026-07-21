
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsProjectMVC.Models.Db;

public class WeatherViewComponent : ViewComponent
{
    private readonly MyNewsContext _context;

    public WeatherViewComponent(MyNewsContext context)
    {
        _context = context;
    }

    // GET: MENUS
    public async Task<IViewComponentResult> InvokeAsync()
    {
       
        return View();
    }


}
