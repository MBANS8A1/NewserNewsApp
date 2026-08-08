
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
        ViewBag.Tags = new SelectList(await _context.Tags.ToListAsync(), "Title", "Title");
        ViewBag.Users = new SelectList(await _context.Users.ToListAsync(), "Id", "FullName");
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

    /// <summary>
    /// Handles asynchronous image uploads specifically for the CKEditor 5 WYSIWYG editor.
    /// This endpoint receives a file via a POST request, saves it to a designated folder
    /// within the web root (wwwroot), and returns a JSON object containing the public URL
    /// of the uploaded file, which CKEditor then uses to display the image.
    /// </summary>
    /// <param name="upload">
    /// The uploaded file, sent from the client-side CKEditor upload adapter.
    /// The parameter name 'upload' must match the key used in the FormData object on the client side.
    /// </param>
    /// <returns>
    /// On success, an JsonResult with a 'url' property (e.g., { "url": "/images/news/some-guid.jpg" }).
    /// On failure (e.g., no file uploaded), a BadRequest result.
    /// </returns>
    [HttpPost]
    // Disables the anti-forgery token validation for this specific action.
    // This is often necessary for AJAX-based file uploaders like CKEditor's, which may not
    // automatically send the token. Use with caution and consider alternative security measures if needed.
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> UploadImage(IFormFile upload)
    {
        // 1. VALIDATION: Check if a file was actually sent and has content.
        if (upload?.Length > 0)
        {
            // 2. FILE NAMING: Generate a unique filename to prevent conflicts and overwriting existing files.
            // Using a GUID is a standard practice for ensuring uniqueness.
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(upload.FileName)}";

            // 3. PATH CONSTRUCTION: Define the storage path for the image.
            // The relative path within the 'wwwroot' directory. This keeps uploads organized.
            var folderPath = Path.Combine("images", "news");
            // The absolute physical path on the server where the file will be saved.
            var saveDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderPath);

            // 4. DIRECTORY CREATION: Ensure the target directory exists. If not, create it.
            // This prevents exceptions if the folder structure is not already in place.
            Directory.CreateDirectory(saveDir);

            // 5. FILE SAVING: Create the full path for the new file and save it to the disk.
            var filePath = Path.Combine(saveDir, fileName);
            // Use an async file stream to write the file. 'await using' ensures proper disposal.
            await using var stream = new FileStream(filePath, FileMode.Create);
            // Copy the contents of the uploaded file to the destination stream.
            await upload.CopyToAsync(stream);

            // 6. RESPONSE: Construct the public URL that the browser will use to access the image.
            // It's crucial that this path is relative to the web root.
            // Replace backslashes with forward slashes for URL compatibility.
            var url = $"/{folderPath.Replace("\\", "/")}/{fileName}";

            // Return a JSON object in the format expected by the CKEditor upload adapter.
            // The client-side script is specifically looking for this 'url' property.
            return Json(new { url });
        }

        // If no file was provided in the request, return an HTTP 400 Bad Request response.
        return BadRequest(new { message = "No file uploaded or file is empty." });
    }

    private bool NewsExists(int? id)
    {
        return _context.News.Any(e => e.Id == id);
    }
}
