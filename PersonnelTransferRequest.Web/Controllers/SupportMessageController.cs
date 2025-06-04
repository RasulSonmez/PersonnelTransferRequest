using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonnelTransferRequest.Entities.Enums;
using PersonnelTransferRequest.Entities.Models;
using PersonnelTransferRequest.Web.Data;
using PersonnelTransferRequest.Web.ViewModels;
using Serilog;
using System.Security.Claims;

namespace PersonnelTransferRequest.Web.Controllers
{
    [Authorize]
    public class SupportMessageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<HomeController> _logger;

        public SupportMessageController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<HomeController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }


        //action method to display the list of support messages for the logged-in user
        [Route("destek-kaydi")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            //Check if the user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                _logger.LogWarning("Yetkisiz erişim denemesi Index sayfasına. Kullanıcı kimliği yok.");
                return Forbid("Yetkisiz erişim.");
            }

            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Index sayfası: Kullanıcı ID alınamadı.");
                return Forbid("Kullanıcı bilgisi alınamadı.");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || user.IsDelete)
            {
                _logger.LogWarning($"Index sayfası: Kullanıcı bulunamadı veya silinmiş. UserId: {userId}");
                return NotFound("Kullanıcı bulunamadı.");
            }

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

            _logger.LogInformation($"Kullanıcı {userId} için {page} sayfasındaki mesajlar getirildi. Toplam mesaj: {totalCount}");


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
                Log.Warning("Details action: id parametresi null gönderildi.");
                return RedirectToAction("Error", "Home", new { code = 400 });
            }

            var supportMessage = await _context.SupportMessages.Include(a => a.SupportMessageReplies)
                .FirstOrDefaultAsync(m => m.Id == id);

            var userId = User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier)?.Value;

            if (supportMessage == null || supportMessage.CreatedById != userId)
            {
                Log.Warning("Details action: Destek mesajı bulunamadı veya kullanıcı yetkisi yok. Id: {Id}, KullanıcıId: {UserId}", id, userId);
                return RedirectToAction("Error", "Home", new { code = 400 });
            }
            Log.Information("Details action: Destek mesajı görüntülendi. Id: {Id}, KullanıcıId: {UserId}", id, userId);

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
                    Log.Warning("Create action: Kullanıcı doğrulanamadı.");
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

                    Log.Information("Create action: Yeni destek mesajı kaydedildi. KullanıcıId: {UserId}, MesajId: {MessageId}", userId, model.Id);

                    TempData["SuccessMessage"] = "Destek mesajınız başarıyla gönderildi.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    Log.Warning("Create action: ModelState geçersiz. Hatalar: {Errors}",
                        string.Join("; ", ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)));
                }

                return View(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Create action: Destek mesajı kaydedilirken hata oluştu.");
                TempData["ErrorMessage"] = "Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.";
                return View(model);
            }
        }


    }
}
