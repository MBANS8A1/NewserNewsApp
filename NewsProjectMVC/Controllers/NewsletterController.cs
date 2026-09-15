using Microsoft.AspNetCore.Mvc;

namespace NewsProjectMVC.Controllers
{
    public class NewsletterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
