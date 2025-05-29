using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonnelTransferRequest.Entities.Models;
using PersonnelTransferRequest.Web.Areas.Admin.ViewModel;
using PersonnelTransferRequest.Web.Data;
using PersonnelTransferRequest.Web.Services.DataTable;

namespace PersonnelTransferRequest.Web.Areas.Admin.Controllers
{
 
    public class DashboardController : AdminBaseController
    {
        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IDataTableService dataTableService)
           : base(context, userManager, dataTableService)
        {
        }

        public IActionResult Index()
        {

            //getting counts for dashboard
            var viewModel = new DashboardViewModel
            {
                PersonnelCount = _context.Users.Count(),
                TransferRequestCount = _context.TransferRequests.Where(a => a.DeletedAt == null).Count(),
                TitleCount = _context.Titles.Where(a => a.DeletedAt == null).Count(),

                TransferRequests = _context.TransferRequests.Include(a => a.Preferences)
                                  .Where(a => a.DeletedAt == null)
                                  .OrderByDescending(a => a.CreatedAt)
                                  .Take(5)
                                  .ToList()

            };

            return View(viewModel);
        }
    }
}
