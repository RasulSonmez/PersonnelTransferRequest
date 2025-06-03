using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PersonnelTransferRequest.Entities.Models;
using PersonnelTransferRequest.Web.Data;
using PersonnelTransferRequest.Web.Services.DataTable;

namespace PersonnelTransferRequest.Web.Areas.Admin.Controllers
{
   
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminBaseController : Controller
    {
        protected readonly ApplicationDbContext _context;
        protected readonly UserManager<ApplicationUser>? _userManager;
        protected readonly IDataTableService? _dataTableService;
        protected readonly SignInManager<ApplicationUser>? _signInManager;
        protected readonly RoleManager<IdentityRole>? _roleManager;

        public AdminBaseController(ApplicationDbContext context,
                                   UserManager<ApplicationUser>? userManager = null,
                                   IDataTableService? dataTableService = null,
                                   SignInManager<ApplicationUser>? signInManager = null, RoleManager<IdentityRole>? roleManager = null) 
        {
            _context = context;
            _userManager = userManager;
            _dataTableService = dataTableService;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


    }
}
