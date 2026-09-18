using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsProjectMVC.Models.Db;
using System.Security.Claims;

namespace NewsProjectMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly MyNewsContext _context; 

        public AuthController(MyNewsContext context)
        {
            _context = context;
        }

        // GET: /Auth/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl; //dictionary key-value pair passed to the View
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                // Find the user by username
                var user = await _context.Users
                    .FirstOrDefaultAsync(user => user.Username == model.Username);

                if (user != null && user.Password == model.Password)
                {
                    if (!user.IsActive)
                    {
                        ModelState.AddModelError(string.Empty, "Your account is disabled.");
                        return View(model);
                    }

                    // --- Create Claims ---
                    // Claims will be the pieces of information about the user.
                    var claims = new List<Claim>
                    {
                      new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                      new Claim(ClaimTypes.Name, user.Username),
                      new Claim("FullName", user.FullName),
                      // Add IsAdmin as a Role claim. This is crucial for authorization.
                      new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
                    };

                    var ukZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/London");
                    DateTime ukDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ukZone);

                    // --- Create Identity & Principal ---
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        // Allows the cookie to persist across browser sessions if "Remember Me" is checked.
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = model.RememberMe ? ukDateTime.AddDays(30) : (DateTimeOffset?)null
                    };

                    // --- Sign In ---
                    // This creates the encrypted authentication cookie and adds it to the response.
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    // Redirect to the originally requested URL, or to the admin dashboard if none was specified.
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {

                        if (user.IsAdmin)
                        {
                            return RedirectToAction("Index", "Home", new { area = "Admin" });
                        }
                        else
                        {
                            return RedirectToAction("Index", "News", new { area = "Admin" });
                        }
                    }
                }

                // If login fails, add a generic error message. (If the user and/or password are invalid)
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
            }

            // If model state is invalid, return the view with the validation messages.
            return View(model);
        }

        // GET: /Auth/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // This clears the authentication cookie.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
