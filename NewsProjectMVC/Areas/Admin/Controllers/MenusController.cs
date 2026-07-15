
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsProjectMVC.Models.Db;

[Area("Admin")]
public class MenusController : Controller
{
    private readonly MyNewsContext _context;

    public MenusController(MyNewsContext context)
    {
        _context = context;
    }

    // GET: MENUS
    public async Task<IActionResult> Index(int? parentId)    
    {
        if (parentId != null)
        {
            // Find the parent menu item to display its details (e.g., its title).
            var parent = await _context.Menus.FirstOrDefaultAsync(menuItem => menuItem.Id == parentId);
            //If single sub-menu item could not be found
            if (parent == null)
            {
                // If the parent menu doesn't exist, return a 404 Not Found error.
                return NotFound();
            }

            // Pass the parent menu object to the view to display its title as a header.
            ViewBag.Parent = parent;
            // Return the view with a list of all menus that have this parent ID.
            return View(await _context.Menus.Where(menuItem => menuItem.ParentId == parentId).ToListAsync());
        }
        return View(await _context.Menus.Where(menuItem => menuItem.ParentId == null).ToListAsync());
    }

    // GET: MENUS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var menu = await _context.Menus
            .FirstOrDefaultAsync(m => m.Id == id);
        if (menu == null)
        {
            return NotFound();
        }

        return View(menu);
    }

    // GET: MENUS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: MENUS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,Link,ParentId,Priority")] Menu menu)
    {
        if (ModelState.IsValid)
        {
            _context.Add(menu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(menu);
    }

    // GET: MENUS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var menu = await _context.Menus.FindAsync(id);
        if (menu == null)
        {
            return NotFound();
        }
        return View(menu);
    }

    // POST: MENUS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Title,Link,ParentId,Priority")] Menu menu)
    {
        if (id != menu.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(menu);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MenuExists(menu.Id))
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
        return View(menu);
    }

    // GET: MENUS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var menu = await _context.Menus
            .FirstOrDefaultAsync(m => m.Id == id);
        if (menu == null)
        {
            return NotFound();
        }

        return View(menu);
    }

    // POST: MENUS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var menu = await _context.Menus.FindAsync(id);
        if (menu != null)
        {
            _context.Menus.Remove(menu);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MenuExists(int? id)
    {
        return _context.Menus.Any(e => e.Id == id);
    }
}
