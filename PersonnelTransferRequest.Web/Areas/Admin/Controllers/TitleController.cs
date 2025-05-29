using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonnelTransferRequest.Entities.Models;
using PersonnelTransferRequest.Web.Data;
using PersonnelTransferRequest.Web.Services.DataTable;
using PersonnelTransferRequest.Web.ViewModels.DataTable;

namespace PersonnelTransferRequest.Web.Areas.Admin.Controllers
{

    public class TitleController : AdminBaseController
    {

        public TitleController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IDataTableService dataTableService)
        : base(context, userManager, dataTableService)
        {
        }

        // Action method to list all titles
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        //Action method to load user data for DataTable
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetAllTitlesForDataTable(DataTableAjaxPostModel model)
        {
            try
            {
                var query = _context.Titles.Where(t=> t.DeletedAt == null).OrderByDescending(t => t.CreatedAt);
                var result = await _dataTableService.GetResultAsync(query, model);
                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something went wrong: " + ex.Message);
            }
        }


        //Action method to show create title form
        public IActionResult Create()
        {
            return View();
        }

        //Action method to show create title form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Title model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                // Duplicate check
                var exists = await _context.Titles
                    .AnyAsync(t => t.TitleName.ToLower() == model.TitleName.ToLower() && t.DeletedAt == null);

                if (exists)
                {
                    ModelState.AddModelError("TitleName", "Bu unvan zaten kayıtlı.");
                    return View(model);
                }

                //remove leading and trailing spaces
                model.TitleName = model.TitleName.Trim();
                await _context.Titles.AddAsync(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Unvan başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.";

                ModelState.AddModelError(string.Empty, "Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz." + ex.Message);
                return View(model);
            }
              
        }

        //Action method to show edit title form
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var title = await _context.Titles.FindAsync(id);
            if (title == null || title.DeletedAt != null)
                return NotFound();

            return View(title);
        }

        //Action method to show edit title form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Title model)
        {
            if (id != model.Id)
                return NotFound();

            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var existingTitle = await _context.Titles.FindAsync(id);
                if (existingTitle == null || existingTitle.DeletedAt != null)
                    return NotFound();

                // Duplicate check (except self)
                bool duplicate = await _context.Titles
                    .AnyAsync(t => t.TitleName.ToLower() == model.TitleName.ToLower() && t.Id != id && t.DeletedAt != null);

                if (duplicate)
                {
                    ModelState.AddModelError("TitleName", "Bu unvan zaten kayıtlı.");
                    return View(model);
                }

                //remove leading and trailing spaces
                existingTitle.TitleName = model.TitleName.Trim();
                existingTitle.ModifiedAt = DateTime.Now;

                _context.Titles.Update(existingTitle);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Unvan başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.";

                ModelState.AddModelError(string.Empty, "Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz." + ex.Message);
                return View(model);
            }
        }


        //Action method to delete a title
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var title = await _context.Titles.FindAsync(id);
            if (title == null)
            {
                return NotFound();
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);

                title.DeletedAt = DateTime.UtcNow;
                //title.DeletedById = user.Id;

                _context.Titles.Update(title);

                await _context.SaveChangesAsync();

                return Json(new { Success = "true" });
            }
            catch (Exception ex)
            {

                return Json(new { Success = "false" });
            }

        }
    }
}
