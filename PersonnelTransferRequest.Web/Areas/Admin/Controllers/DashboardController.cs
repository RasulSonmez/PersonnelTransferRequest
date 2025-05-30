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

       
        /// Displays the main dashboard with personnel and transfer request statistics       
        /// <returns>Dashboard view with statistical data</returns>
        public IActionResult Index()
        {
            // Check if database context or required tables are null to prevent null reference exceptions
            if (_context?.Users == null || _context?.TransferRequests == null || _context?.Titles == null)
            {              
                return View(GetEmptyDashboard());
            }

            // Create view model with dashboard statistics
            var viewModel = new DashboardViewModel
            {
                PersonnelCount = _context.Users.Count(),

                TransferRequestCount = _context.TransferRequests
                    .Where(a => a.DeletedAt == null).Count(),

                TitleCount = _context.Titles
                    .Where(a => a.DeletedAt == null).Count(),
            
                TransferRequests = _context.TransferRequests
                    .Include(a => a.Preferences) 
                    .Where(a => a.DeletedAt == null)  
                    .OrderByDescending(a => a.CreatedAt)  
                    .Take(5)  
                    .ToList()  
            };
           
            return View(viewModel);
        }

      
        /// Creates an empty dashboard view model with default values     
        private DashboardViewModel GetEmptyDashboard()
        {
            return new DashboardViewModel
            {
                PersonnelCount = 0,  
                TransferRequestCount = 0,  
                TitleCount = 0,  
                TransferRequests = new List<TransferRequest>()  
            };
        }
    }
}
