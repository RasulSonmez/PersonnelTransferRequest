using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PersonnelTransferRequest.Entities.Models;
using PersonnelTransferRequest.Web.Data;
using PersonnelTransferRequest.Web.Services.DataTable;

namespace PersonnelTransferRequest.Web.Areas.Admin.Controllers
{
    //TODO:: put "Admin" role 
    [Area("Admin")]
    public class AdminBaseController : Controller
    {
        protected readonly ApplicationDbContext _context;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly IDataTableService _dataTableService;
        public AdminBaseController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IDataTableService dataTableService)
        {
            _context = context;
            _userManager = userManager;
            _dataTableService = dataTableService;
        }
   

    }
}
