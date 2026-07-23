
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsProjectMVC.Models.Db;

public class SettingsController : Controller
{
    private readonly MyNewsContext _context;

    public SettingsController(MyNewsContext context)
    {
        _context = context;
    }

    // GET: SETTINGS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var setting = await _context.Settings.FindAsync(id);
        if (setting == null)
        {
            return NotFound();
        }
        return View(setting);
    }

    // POST: SETTINGS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Title,Address,Email,Phone,Copyright,Facebook,X,Instagram,YouTube,LinkedIn")] Setting setting)
    {
        if (id != setting.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(setting);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SettingExists(setting.Id))
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
        return View(setting);
    }

    // GET: SETTINGS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var setting = await _context.Settings
            .FirstOrDefaultAsync(m => m.Id == id);
        if (setting == null)
        {
            return NotFound();
        }

        return View(setting);
    }

    // POST: SETTINGS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var setting = await _context.Settings.FindAsync(id);
        if (setting != null)
        {
            _context.Settings.Remove(setting);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool SettingExists(int? id)
    {
        return _context.Settings.Any(e => e.Id == id);
    }
}
