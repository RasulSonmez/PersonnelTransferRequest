using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PersonnelTransferRequest.Entities.Enums;
using PersonnelTransferRequest.Entities.Models;
using PersonnelTransferRequest.Web.Data;
using PersonnelTransferRequest.Web.ViewModels;
using Serilog;

namespace PersonnelTransferRequest.Web.Controllers
{
    [Authorize]
    public class TransferRequestController : Controller
    {

        protected readonly ApplicationDbContext _context;
        protected readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<HomeController> _logger;

        public TransferRequestController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<HomeController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }


        //action method to display the list of transfer requests for the logged-in user
        [HttpGet]
        [Route("tayin-taleplerim")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            //Check if the user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                Log.Warning("Tayin talepleri Index: Yetkisiz erişim denemesi.");
                return Forbid("Yetkisiz erişim.");
            }

            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                Log.Warning("Tayin talepleri Index: Kullanıcı bilgisi alınamadı.");
                return Forbid("Kullanıcı bilgisi alınamadı.");
            }


            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || user.IsDelete)
            {
                Log.Warning("Tayin talepleri Index: Kullanıcı bulunamadı veya silinmiş. KullanıcıId: {UserId}", userId);
                return NotFound("Kullanıcı bulunamadı.");
            }

            //Total count of transfer requests for the user
            var totalCount = await _context.TransferRequests
                .Where(tr => tr.ApplicationUserId == userId)
                .CountAsync();

            // If the page number is less than 1 or greater than the total pages, return a bad request
            var requests = await _context.TransferRequests
                .Where(tr => tr.ApplicationUserId == userId)
                .Include(tr => tr.Preferences.OrderBy(p => p.PriorityOrder))
                .OrderByDescending(tr => tr.RequestDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            //check system is open transfer request 
            var setting = await _context.SystemSettings.FirstOrDefaultAsync();
            ViewBag.IsTransferRequestOpen = setting?.IsTransferRequestOpen ?? false;

            //using model to pass data to the view
            var model = new TransferRequestListViewModel
            {
                Requests = requests,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            Log.Information("Tayin talepleri Index: Sayfa {Page}, Sayfa boyutu {PageSize}, Toplam kayıt {TotalCount} ile başarılı getirildi. KullanıcıId: {UserId}",
      page, pageSize, totalCount, userId);

            return View(model);
        }


        //action method to display the form for creating a new transfer request
        public IActionResult _GetTransferRequestPartial()
        {
            return PartialView();
        }

        //action method to create a new transfer request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TransferRequestCreate(TransferRequestCreateViewModel model)
        {
            // Check if the user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                Log.Warning("Transfer talebi oluşturma: Yetkisiz erişim denemesi.");
                return Forbid("Yetkisiz erişim.");
            }

            var userId = _userManager.GetUserId(User);

            // If the user ID is null or empty, return a forbidden response
            if (string.IsNullOrEmpty(userId))
            {
                Log.Warning("Transfer talebi oluşturma: Kullanıcı bilgisi alınamadı.");
                return Forbid("Kullanıcı bilgisi alınamadı.");
            }

            var user = await _userManager.FindByIdAsync(userId);

            // If the user is not found or is marked as deleted, return a not found response
            if (user == null || user.IsDelete)
            {
                Log.Warning("Transfer talebi oluşturma: Kullanıcı bulunamadı veya silinmiş. KullanıcıId: {UserId}", userId);
                return NotFound("Kullanıcı bulunamadı.");
            }


            // custom controllers
            if (model.Preferences == null || model.Preferences.Count == 0)
            {
                Log.Warning("Transfer talebi oluşturma: Tercih yapılmamış. KullanıcıId: {UserId}", userId);

                ModelState.AddModelError("", "En az 1 tercih yapılmalıdır.");
            }

            if (model.Preferences != null)
            {
                if (model.Preferences.Count > 5)
                {
                    Log.Warning("Transfer talebi oluşturma: 5'ten fazla tercih yapılmaya çalışıldı. KullanıcıId: {UserId}, TercihSayısı: {Count}", userId, model.Preferences.Count);

                    ModelState.AddModelError("", "En fazla 5 tercih yapabilirsiniz.");
                }

                if (model.Preferences.Select(p => p.PriorityOrder).Distinct().Count() != model.Preferences.Count)
                {
                    Log.Warning("Transfer talebi oluşturma: Tercih sıraları benzersiz değil. KullanıcıId: {UserId}", userId);

                    ModelState.AddModelError("", "Tercih sıraları benzersiz olmalıdır.");
                }

                if (model.Preferences.Select(p => p.CourtHouse).Distinct().Count() != model.Preferences.Count)
                {
                    Log.Warning("Transfer talebi oluşturma: Aynı adliye birden fazla tercih edilmiş. KullanıcıId: {UserId}", userId);

                    ModelState.AddModelError("", "Aynı adliye birden fazla tercih edilemez.");
                }
            }

            // modalstate isvalid controll
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                Log.Warning("Transfer talebi oluşturma: ModelState geçersiz. Hatalar: {Errors}. KullanıcıId: {UserId}", string.Join(", ", errorList), userId);

                TempData["ModelErrors"] = JsonConvert.SerializeObject(errorList);
                TempData["OpenModal"] = "true";
                return RedirectToAction("Index");
            }

            var request = new TransferRequest
            {
                Description = model.Description?.Trim(),
                RequestDate = DateTime.UtcNow,
                RequestType = model.RequestType,
                Status = TransferStatus.Pending,
                ApplicationUserId = userId,
                CreatedAt = DateTime.UtcNow,
                CreatedById = userId,
                Preferences = model.Preferences.Select(p => new TransferPreference
                {
                    CourtHouse = p.CourtHouse?.Trim() ?? "",
                    PriorityOrder = p.PriorityOrder,
                    CreatedAt = DateTime.UtcNow,
                    CreatedById = userId
                }).ToList()
            };

            try
            {
                _context.TransferRequests.Add(request);
                await _context.SaveChangesAsync();

                Log.Information("Transfer talebi başarıyla oluşturuldu. TransferRequestId: {RequestId}, KullanıcıId: {UserId}", request.Id, userId);

                TempData["SuccessMessage"] = "Tayin talebiniz başarıyla oluşturuldu.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Transfer talebi oluşturma sırasında hata oluştu. KullanıcıId: {UserId}", userId);


                ModelState.AddModelError("", "Birşeyler ters gitti: " + ex.Message);

                var errorList = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                TempData["ModelErrors"] = JsonConvert.SerializeObject(errorList);
                TempData["OpenModal"] = "true";
                return View("Index", model);
            }
        }

    }
}
