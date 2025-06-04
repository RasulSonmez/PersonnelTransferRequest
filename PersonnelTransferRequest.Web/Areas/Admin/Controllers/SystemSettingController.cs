using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonnelTransferRequest.Entities.Models;
using PersonnelTransferRequest.Web.Data;
using PersonnelTransferRequest.Web.Services.DataTable;

namespace PersonnelTransferRequest.Web.Areas.Admin.Controllers
{
    public class SystemSettingController : AdminBaseController
    {

        public SystemSettingController(ApplicationDbContext context)
       : base(context)
        {
        }

        // action for toggle transfer request open or close
        [HttpGet]
        public async Task<IActionResult> _ToggleTransferRequestPartial()
        {
            try
            {
                var setting = await _context.SystemSettings.FirstOrDefaultAsync();

                if (setting == null)
                {
                    setting = new SystemSetting
                    {
                        IsTransferRequestOpen = false
                    };

                    _context.SystemSettings.Add(setting);
                    await _context.SaveChangesAsync();
                }

                return PartialView("_ToggleTransferRequestPartial", setting);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Beklenmeyen hata oluştu: {ex.Message}");
            }
        }

        //action method update transfer request system setting
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTransferRequest(bool isOpen)
        {
            try
            {
                var setting = await _context.SystemSettings.FirstOrDefaultAsync();
                if (setting == null)
                {
                    setting = new SystemSetting { IsTransferRequestOpen = isOpen };
                    _context.SystemSettings.Add(setting);
                }
                else
                {
                    setting.IsTransferRequestOpen = isOpen;
                    _context.SystemSettings.Update(setting);
                }

                await _context.SaveChangesAsync();

                string message = isOpen
                    ? "Tayin talepleri açıldı."
                    : "Tayin talepleri kapatıldı.";

                return Ok(new { success = true, message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Bir hata oluştu: " + ex.Message });
            }
        }


    }
}
