using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PersonnelTransferRequest.Entities.Models;
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
            return View();
        }
    }
}
