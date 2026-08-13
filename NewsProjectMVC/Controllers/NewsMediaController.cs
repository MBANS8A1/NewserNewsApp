using Microsoft.AspNetCore.Mvc;
using NewsProjectMVC.Models.Db;

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
    }
}
