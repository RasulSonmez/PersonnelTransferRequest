using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonnelTransferRequest.Common.Extensions;
using PersonnelTransferRequest.Entities.Enums;
using PersonnelTransferRequest.Entities.Models;
using PersonnelTransferRequest.Web.Areas.Admin.ViewModel;
using PersonnelTransferRequest.Web.Data;
using PersonnelTransferRequest.Web.Services.DataTable;
using PersonnelTransferRequest.Web.ViewModels.DataTable;
using System.Security.Claims;

namespace PersonnelTransferRequest.Web.Areas.Admin.Controllers
{
    public class SupportMessageController : AdminBaseController
    {
        public SupportMessageController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IDataTableService dataTableService)
       : base(context, userManager, dataTableService)
        {
        }


        // Action method to list all titles
        [HttpGet]
        public IActionResult Index()
        {
            //data comes from dynamic data table GetAllSupportMessageForDataTable
            return View();
        }

        //Action method to load user data for DataTable
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetAllSupportMessageForDataTable(DataTableAjaxPostModel model)
        {
            try
            {
                var query = _context.SupportMessages
         .Where(sm => sm.DeletedAt == null)
         .Select(sm => new
         {
             sm.Id,
             sm.CreatedAt,
             sm.Title,
             sm.Status
         });

                var list = await query.ToListAsync();

                var resultList = list.Select(sm => new SupportMessageDataTableViewModel
                {
                    Id = sm.Id,
                    CreatedAt = sm.CreatedAt,
                    Title = sm.Title.Length > 30 ? sm.Title.Substring(0, 30) + "..." : sm.Title,
                    Status = sm.Status.HasValue ? sm.Status.Value.GetDisplayName() : "Bilinmiyor"
                });

                var result = await _dataTableService.GetResultAsync(resultList.AsQueryable(), model);
                return Json(result);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Bir şeyler ters gitti: " + ex.Message);
            }
        }


        //action method  support deails
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID bulunamadı.";
                return RedirectToAction("Error", "Home", new { code = 400 });
            }

            var supportMessage = await _context.SupportMessages
                .Include(a => a.SupportMessageReplies)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (supportMessage == null)
            {
                return RedirectToAction("Error", "Home", new { code = 400 });
            }

            var user = await _context.Users
      .FirstOrDefaultAsync(u => u.Id == supportMessage.CreatedById);

            ViewBag.User = user;


            return View(supportMessage);
        }


        //action method reply support message
        [HttpPost]
        public IActionResult ReplySupportMessage(int id, string message)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier)?.Value;

                var reply = new SupportMessageReply
                {
                    Message = message,
                    CreatedById = userId,
                    SupportMessageId = id
                };

                _context.Add(reply);

                var supportMessage = _context.SupportMessages.Find(id);
                if (supportMessage != null)
                {
                    supportMessage.Status = SupportStatus.Answered;
                }

                _context.SaveChanges();

                return Json(new { Success = true, Message = "Mesajınız başarıyla gönderildi." });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "İşlem sırasında bir hata oluştu: " + ex.Message });
            }
        }



    }
}
