
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsProjectMVC.Models.Db;
using System.Security.Claims;

[Area("Admin")]
public class NewsController : Controller
{
    private readonly MyNewsContext _context;

    public NewsController(MyNewsContext context)
    {
        _context = context;
    }

    // GET: NEWSS
    public IActionResult Index()    
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> LoadNewsData()
    {
        try
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            IQueryable<News> query = _context.News.AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                //var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value.ToString());
                //query = query.Where(news => news.UserId == userId);
            }

            var recordsTotal = await query.CountAsync();

            if (!string.IsNullOrEmpty(searchValue))
            {
                var searchValueLower = searchValue.ToLower();
                query = query.Where(n =>
                    n.Title.ToLower().Contains(searchValueLower) ||
                    n.Status.ToLower().Contains(searchValueLower));
            }

            var recordsFiltered = await query.CountAsync();

            
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                if (sortColumnDirection == "asc")
                {
                    switch (sortColumn.ToLower())
                    {
                        case "title":
                            query = query.OrderBy(n => n.Title);
                            break;
                        case "viewcount":
                            query = query.OrderBy(n => n.ViewCount);
                            break;
                            // Add other columns here
                    }
                }
                else
                {
                    switch (sortColumn.ToLower())
                    {
                        case "title":
                            query = query.OrderByDescending(n => n.Title);
                            break;
                        case "viewcount":
                            query = query.OrderByDescending(n => n.ViewCount);
                            break;
                            // Add other columns here
                    }
                }
            }

            var pagedData = await query.Skip(skip).Take(pageSize).ToListAsync();

            var jsonData = new
            {
                draw = draw,
                recordsFiltered = recordsFiltered,
                recordsTotal = recordsTotal,
                data = pagedData
            };

            return Ok(jsonData);
        }
        catch (Exception ex)
        {
            // Log the exception
            return BadRequest();
        }
    }


    // GET: NEWSS/Create
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Title");
        return View();
    }

    // POST: NEWSS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,ShortDescription,LongDescription,CreatedAt,ViewCount,Status,ImageName,CategoryId,Tags,UserId")] News news)
    {
        if (ModelState.IsValid)
        {
            _context.Add(news);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(news);
    }

    // GET: NEWSS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var news = await _context.News.FindAsync(id);
        if (news == null)
        {
            return NotFound();
        }
        return View(news);
    }

    // POST: NEWSS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Title,ShortDescription,LongDescription,CreatedAt,ViewCount,Status,ImageName,CategoryId,Tags,UserId")] News news)
    {
        if (id != news.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(news);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NewsExists(news.Id))
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
        return View(news);
    }

    // GET: NEWSS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var news = await _context.News
            .FirstOrDefaultAsync(m => m.Id == id);
        if (news == null)
        {
            return NotFound();
        }

        return View(news);
    }

    // POST: NEWSS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var news = await _context.News.FindAsync(id);
        if (news != null)
        {
            _context.News.Remove(news);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool NewsExists(int? id)
    {
        return _context.News.Any(e => e.Id == id);
    }
}
