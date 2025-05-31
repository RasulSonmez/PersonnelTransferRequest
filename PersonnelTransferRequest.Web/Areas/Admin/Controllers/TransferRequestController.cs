using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonnelTransferRequest.Entities.Enums;
using PersonnelTransferRequest.Entities.Models;
using PersonnelTransferRequest.Web.Data;
using PersonnelTransferRequest.Web.Services.DataTable;
using PersonnelTransferRequest.Web.ViewModels.DataTable;

namespace PersonnelTransferRequest.Web.Areas.Admin.Controllers
{
 
    public class TransferRequestController : AdminBaseController
    {

        public TransferRequestController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IDataTableService dataTableService)
            : base(context, userManager, dataTableService)
        {
        }

        // Action method to list all titles
        [HttpGet]
        public IActionResult Index()
        {
            //data comes from dynamic data table GetAllTitlesForDataTable
            return View();
        }

        //Action method to load transfer request data for DataTable
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetAllTransferRequestForDataTable(DataTableAjaxPostModel model)
        {
            try
            {
                var query = _context.TransferRequests
             .Include(t => t.Preferences)
             .Include(t => t.ApplicationUser)
             .Where(t => t.DeletedAt == null)
             .Select(t => new {
                 id = t.Id,
                 requestDate = t.RequestDate,              
                 applicationUser_RegistrationNumber = t.ApplicationUser.RegistrationNumber,
                 applicationUser_Name = t.ApplicationUser.Name,
                 applicationUser_Surname = t.ApplicationUser.Surname,
                 applicationUser_Gsm = t.ApplicationUser.GSM,               
             })
             .OrderByDescending(t => t.requestDate);
                var result = await _dataTableService.GetResultAsync(query, model);
               
                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Birşeyler ters gitti: " + ex.Message);
            }
        }

        //Action method to show transfer request details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID bulunamadı.";
                return RedirectToAction("Error", "Home", new { code = 400 });
            }


            var transferRequest = await _context.TransferRequests
                .Include(tr => tr.Preferences)
                .Include(tr => tr.ApplicationUser)
                .FirstOrDefaultAsync(tr => tr.Id == id);

            if (transferRequest == null)
            {
                return NotFound();
            }

            return View(transferRequest);
        }

        //Action method to update transfer request status
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int Id, TransferStatus Status)
        {
            var transferRequest = await _context.TransferRequests.FindAsync(Id);

            if (transferRequest == null)
            {
                TempData["ErrorMessage"] = "Tayin talebi bulunamadı.";
                return RedirectToAction(nameof(Details), new { id = Id });
            }

            transferRequest.Status = Status;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tayin talebi başarıyla güncellendi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Bir hata oluştu: " + ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id = Id });
        }

        //Action method to delete a title
        [HttpGet]      
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID bulunamadı.";
                return RedirectToAction("Error", "Home", new { code = 400 });
            }

            var transferRequest = await _context.TransferRequests
                .Include(tr => tr.Preferences) 
                .FirstOrDefaultAsync(tr => tr.Id == id);

            if (transferRequest == null)
            {
                TempData["ErrorMessage"] = "Tayin talebi bulunamadı.";
                return RedirectToAction("Error", "Home", new { code = 404 });
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    TempData["ErrorMessage"] = "Kullanıcı doğrulanamadı. Lütfen tekrar giriş yapınız.";
                    return RedirectToAction("Error", "Home", new { code = 401 });
                }

                // Soft delete TransferRequest
                transferRequest.DeletedAt = DateTime.UtcNow;
                transferRequest.DeletedById = user.Id;

                //If TransferPreference has soft delete
                if (transferRequest.Preferences != null)
                {
                    foreach (var pref in transferRequest.Preferences)
                    {
                        pref.DeletedAt = DateTime.UtcNow;
                        pref.DeletedById = user.Id;
                        _context.TransferPreferences.Update(pref);
                    }
                }

                _context.TransferRequests.Update(transferRequest);

                await _context.SaveChangesAsync();

                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "İşlem sırasında bir hata oluştu.";
                return RedirectToAction("Error", "Home", new { code = 500 });
            }
        }

    }
}
