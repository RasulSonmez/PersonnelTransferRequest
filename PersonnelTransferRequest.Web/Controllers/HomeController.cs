using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PersonnelTransferRequest.Entities.Models;
using PersonnelTransferRequest.Web.Data;
using PersonnelTransferRequest.Web.Models;
using System.Diagnostics;

namespace PersonnelTransferRequest.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<HomeController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(MyProfile));
        }

        //The action method required for the Personnel profile page
        [Route("profil")]
        public async Task<IActionResult> MyProfile()
        {
            _logger.LogInformation("MyProfile action baþlatýldý.");

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                _logger.LogWarning("User bulunamadý ve logine yönlendirildi.");

                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            _logger.LogInformation("userId için kullanýcý profili alýndý: {UserId}", user.Id);

            return View(user);

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? code = null)
        {
            _logger.LogError("Error sayfasý hata kodu {StatusCode} verdi, RequestId: {RequestId}", code, Activity.Current?.Id ?? HttpContext.TraceIdentifier);

            var viewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = code
            };

            return View(viewModel);
        }
    }
}
