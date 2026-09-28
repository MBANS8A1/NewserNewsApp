using Microsoft.AspNetCore.Mvc;

namespace NewsProjectMVC.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HandleErrors(int statusCode)
        {
            if(statusCode == 403)
                {
                    Response.StatusCode = 403;
                    return RedirectToAction("AccessDenied", "Auth");
                }
            if(statusCode == 500)
                {
                    Response.StatusCode = 500;
                    @ViewData["internalServerErrorCode"] = Response.StatusCode;
                    return View("ServerError");
                }
            Response.StatusCode = 404;
            @ViewData["notFoundRespCode"] = Response.StatusCode;

            return View("NotFound");
        }
    }
}
