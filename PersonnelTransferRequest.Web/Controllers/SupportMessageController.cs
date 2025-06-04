using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonnelTransferRequest.Entities.Enums;
using PersonnelTransferRequest.Entities.Models;
using PersonnelTransferRequest.Web.Data;
using PersonnelTransferRequest.Web.ViewModels;
using System.Security.Claims;

namespace PersonnelTransferRequest.Web.Controllers
{
    [Authorize]
    public class SupportMessageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SupportMessageController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //action method to display the list of support messages for the logged-in user
        [Route("destek-kaydi")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            //Check if the user is authenticated
            if (!User.Identity.IsAuthenticated)
                return Forbid("Yetkisiz erişim.");

            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
                return Forbid("Kullanıcı bilgisi alınamadı.");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || user.IsDelete)
                return NotFound("Kullanıcı bulunamadı.");

            //Total count of transfer requests for the user
            var totalCount = await _context.SupportMessages
                .Where(tr => tr.CreatedById == userId)
                .CountAsync();

            // If the page number is less than 1 or greater than the total pages, return a bad request
            var messages = await _context.SupportMessages
                .Where(tr => tr.CreatedById == userId)              
                .OrderByDescending(tr => tr.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            //check system is open transfer request 
            var setting = await _context.SystemSettings.FirstOrDefaultAsync();
            ViewBag.IsTransferRequestOpen = setting?.IsTransferRequestOpen ?? false;

            //using model to pass data to the view
            var model = new SupportMessagetListViewModel
            {
                SupportMessages = messages,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return View(model);
        }

        //action method to display the details of a specific support message
        [Route("destek-detay")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error", "Home", new { code = 400 });
            }

            var supportMessage = await _context.SupportMessages.Include(a => a.SupportMessageReplies)
                .FirstOrDefaultAsync(m => m.Id == id);

            var userId = User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier)?.Value;

            if (supportMessage == null || supportMessage.CreatedById != userId)
            {
                return RedirectToAction("Error", "Home", new { code = 400 });
            }

            return View(supportMessage);
        }

        //action method to display the form for creating a new support message
        public IActionResult Create()
        {
            return View();
        }

        //action method to handle the form submission for creating a new support message
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupportMessage model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    TempData["ErrorMessage"] = "Kullanıcı doğrulanamadı. Lütfen tekrar giriş yapınız.";
                    return RedirectToAction("Error", "Home", new { code = 401 });
                }

                if (ModelState.IsValid)
                {
                    var userId = User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier)?.Value;

                    model.CreatedById = userId;
                    model.Status = SupportStatus.New;
                    _context.Add(model);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Destek mesajınız başarıyla gönderildi.";
                    return RedirectToAction(nameof(Index));
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.";
                return View(model);
            }
        }


    }
}
