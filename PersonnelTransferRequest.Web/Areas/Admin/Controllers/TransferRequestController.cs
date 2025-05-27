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

        [HttpPost]
        public async Task<IActionResult> Index(DataTableAjaxPostModel model)
        {
            var query = _context.TransferRequests
                .Include(x => x.ApplicationUser)
                .Include(x => x.Preferences)
                .Where(x => x.DeletedAt == null);

            var result = await _dataTableService.GetResultAsync(query, model);

            return Json(result);
        }
    }
}
