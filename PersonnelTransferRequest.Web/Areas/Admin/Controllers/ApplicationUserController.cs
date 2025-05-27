using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonnelTransferRequest.Entities.Models;
using PersonnelTransferRequest.Web.Data;
using PersonnelTransferRequest.Web.Services.DataTable;
using PersonnelTransferRequest.Web.ViewModels.DataTable;

namespace PersonnelTransferRequest.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ApplicationUserController : AdminBaseController
    {
     

       public ApplicationUserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IDataTableService dataTableService)
        :  base(context, userManager, dataTableService)
        {
        }

        //Action method to list all users
        [HttpPost]
        public async Task<IActionResult> Index([FromBody] DataTableAjaxPostModel model)
        {
            try
            {
                var query = _context.Users
                    .Where(u => !u.IsDelete);

                var result = await _dataTableService.GetResultAsync(query, model);

                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something went wrong: " + ex.Message);
            }
        }

        //Action method to show user details
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.Users
                .OfType<ApplicationUser>()
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDelete);

            if (user == null)
            {
                return NotFound();
            }
        
            return View(user);
        }


        //Action method to delete a user
        [HttpGet]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                user.IsDelete = true;
                _context.Users.Update(user);

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
