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
    public class TransferRequestController : AdminBaseController
    {

        public TransferRequestController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IDataTableService dataTableService)
            : base(context, userManager, dataTableService)
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> LoadData(DataTableAjaxPostModel model)
        {
            try
            {
                var query = _context.Users.Where(u => !u.IsDelete);
                var result = await _dataTableService.GetResultAsync(query, model);
                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something went wrong: " + ex.Message);
            }
        }

    }
}
