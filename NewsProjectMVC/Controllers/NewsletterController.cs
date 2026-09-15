using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsProjectMVC.Models.Db;
using System.Text.RegularExpressions;
namespace NewsProjectMVC.Controllers
{
    public class NewsletterController : Controller
    {
        private readonly MyNewsContext _context;

        public NewsletterController(MyNewsContext context)
        {
            _context = context;
        }


        [HttpPost]
        public async Task<IActionResult> Subscribe(string email)
        {
            // Server-side validation for email format
            if (string.IsNullOrEmpty(email) || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return Json(new { success = false, message = "Please enter a valid email address." });
            }

            // Check for duplicate email
            var isAlreadySubscribed = await _context.Subscribers.AnyAsync(subscriber => subscriber.Email == email.ToLower());
            if (isAlreadySubscribed)
            {
                return Json(new { success = false, message = "This email address is already on the subscribed list!" });
            }

            // Create a new subscriber object
            var ukZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/London");
            DateTime ukDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ukZone);
            var subscriber = new Subscriber
            {
                Email = email.ToLower(),
                SubscribedAt = ukDateTime,
                IsActive = true
            };

            // Add to database and save
            _context.Subscribers.Add(subscriber);
            await _context.SaveChangesAsync();

            // Return a success response
            return Json(new { success = true, message = "Thank you for subscribing to the news list!" });
        }
    }
}
