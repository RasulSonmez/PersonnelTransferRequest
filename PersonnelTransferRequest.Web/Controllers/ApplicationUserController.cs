using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PersonnelTransferRequest.Entities.Models;
using PersonnelTransferRequest.Web.Data;

namespace PersonnelTransferRequest.Web.Controllers
{
    public class ApplicationUserController : Controller
    {

        protected readonly ApplicationDbContext _context;
        protected readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //The action method required for the Personnel profile page
        [Route("profil")]
        public async Task<IActionResult> MyProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                //If the user does not exist in the system, redirect to the login page or show an error message.
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            return View(user);

        }
    }
}
