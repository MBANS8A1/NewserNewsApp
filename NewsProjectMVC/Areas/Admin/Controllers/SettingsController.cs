
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsProjectMVC.Models.Db;

[Area("Admin")]
public class SettingsController : Controller
{
    private readonly MyNewsContext _context;

    public SettingsController(MyNewsContext context)
    {
        _context = context;
    }

    // GET: SETTINGS/Edit/5
    public async Task<IActionResult> Edit()
    {
        var setting = await _context.Settings.FirstOrDefaultAsync();
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
    public async Task<IActionResult> Edit(int? id, Setting setting)
    {

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
            return RedirectToAction("Edit");
        }
        return View(setting);
    }

    private bool SettingExists(int? id)
    {
        return _context.Settings.Any(e => e.Id == id);
    }
}
