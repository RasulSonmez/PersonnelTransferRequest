using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonnelTransferRequest.Entities.Models;
using PersonnelTransferRequest.Web.Data;
using PersonnelTransferRequest.Web.Services.DataTable;
using PersonnelTransferRequest.Web.ViewModels.DataTable;

namespace PersonnelTransferRequest.Web.Areas.Admin.Controllers
{
 
    public class TransferRequestController : AdminBaseController
    {

        public TransferRequestController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IDataTableService dataTableService)
            : base(context, userManager, dataTableService)
        {
        }

      

       

    }
}
