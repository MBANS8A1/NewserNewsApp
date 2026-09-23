
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsProjectMVC.Models.Db;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class SubscribersController : Controller
{
    private readonly MyNewsContext _context;

    public SubscribersController(MyNewsContext context)
    {
        _context = context;
    }

    // GET: SUBSCRIBERS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Subscribers.ToListAsync());
    }

    // GET: SUBSCRIBERS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var subscriber = await _context.Subscribers
            .FirstOrDefaultAsync(m => m.Id == id);
        if (subscriber == null)
        {
            return NotFound();
        }

        return View(subscriber);
    }

    // GET: SUBSCRIBERS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: SUBSCRIBERS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Email,SubscribedAt,IsActive")] Subscriber subscriber)
    {
        if (ModelState.IsValid)
        {
            _context.Add(subscriber);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(subscriber);
    }

    // GET: SUBSCRIBERS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var subscriber = await _context.Subscribers.FindAsync(id);
        if (subscriber == null)
        {
            return NotFound();
        }
        return View(subscriber);
    }

    // POST: SUBSCRIBERS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Email,SubscribedAt,IsActive")] Subscriber subscriber)
    {
        if (id != subscriber.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(subscriber);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubscriberExists(subscriber.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(subscriber);
    }

    // GET: SUBSCRIBERS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var subscriber = await _context.Subscribers
            .FirstOrDefaultAsync(m => m.Id == id);
        if (subscriber == null)
        {
            return NotFound();
        }

        return View(subscriber);
    }

    // POST: SUBSCRIBERS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var subscriber = await _context.Subscribers.FindAsync(id);
        if (subscriber != null)
        {
            _context.Subscribers.Remove(subscriber);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool SubscriberExists(int? id)
    {
        return _context.Subscribers.Any(e => e.Id == id);
    }
}
