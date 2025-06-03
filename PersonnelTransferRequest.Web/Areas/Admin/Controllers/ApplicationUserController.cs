using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using PersonnelTransferRequest.Common.Extensions;
using PersonnelTransferRequest.Common.Helper.EmailHelper;
using PersonnelTransferRequest.Entities.Models;
using PersonnelTransferRequest.Web.Areas.Admin.ViewModel;
using PersonnelTransferRequest.Web.Data;
using PersonnelTransferRequest.Web.Services.DataTable;
using PersonnelTransferRequest.Web.ViewModels.DataTable;
using System.Text;
using System.Text.Encodings.Web;

namespace PersonnelTransferRequest.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ApplicationUserController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IDataTableService _dataTableService;
        private readonly IMailService _mailService;
        public ApplicationUserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IDataTableService dataTableService, IMailService mailService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _dataTableService = dataTableService;
            _mailService = mailService;
        }


        //Action method to list all users
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Index()
        {
            //data comes from dynamic data table GetAllTitlesForDataTable
            return View();
        }

        //Action method to load user data for DataTable
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetAllUserForDataTable(DataTableAjaxPostModel model)
        {
            try
            {
                var query = _context.Users.Where(u => !u.IsDelete && u.IsAdmin == false);
                var result = await _dataTableService.GetResultAsync(query, model);
                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Birşeyler ters gitti: " + ex.Message);
            }
        }

        //Action method to show create user form
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Titles"] = new SelectList(_context.Titles
    .Where(t => t.DeletedAt == null && !string.IsNullOrEmpty(t.TitleName))
    .OrderByDescending(t => t.CreatedAt), "TitleName", "TitleName");

            return View();
        }


        //Action method to show create user form
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationUser model, string password)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewData["Titles"] = new SelectList(_context.Titles
                        .Where(t => t.DeletedAt == null && !string.IsNullOrEmpty(t.TitleName))
                        .OrderByDescending(t => t.CreatedAt), "TitleName", "TitleName");

                    return View(model);
                }

                // Check for existing user with the same TCKN
                if (!string.IsNullOrEmpty(model.TCKN))
                {
                    var existingUser = await _userManager.Users
                        .FirstOrDefaultAsync(u => u.TCKN == model.TCKN && !u.IsDelete);

                    if (existingUser != null)
                    {
                        ModelState.AddModelError("TCKN", "TCKN doğru olduğuna emin olunuz.");

                        ViewData["Titles"] = new SelectList(_context.Titles
                  .Where(t => t.DeletedAt == null && !string.IsNullOrEmpty(t.TitleName))
                  .OrderByDescending(t => t.CreatedAt), "TitleName", "TitleName");

                        return View(model);
                    }
                }

                // Check for existing user with the same RegistrationNumber
                if (!string.IsNullOrEmpty(model.RegistrationNumber))
                {
                    var existingUser = await _userManager.Users
                        .FirstOrDefaultAsync(u => u.RegistrationNumber == model.RegistrationNumber && !u.IsDelete);

                    if (existingUser != null)
                    {
                        ModelState.AddModelError("RegistrationNumber", "Bu sicil Numarası alınmış zaten");

                        ViewData["Titles"] = new SelectList(_context.Titles
                  .Where(t => t.DeletedAt == null && !string.IsNullOrEmpty(t.TitleName))
                  .OrderByDescending(t => t.CreatedAt), "TitleName", "TitleName");

                        return View(model);
                    }
                }


                // Upload profile image using custom extension
                if (model.ProfilPhotoFile != null)
                {
                    var uploadedPath = UploadImageExtension.UploadProfileImage(model.ProfilPhotoFile);

                    if (string.IsNullOrEmpty(uploadedPath))
                    {
                        ModelState.AddModelError("ProfilPhotoFile", "Yüklenen dosya geçersiz veya yüklenemedi.");

                        ViewData["Titles"] = new SelectList(_context.Titles
                  .Where(t => t.DeletedAt == null && !string.IsNullOrEmpty(t.TitleName))
                  .OrderByDescending(t => t.CreatedAt), "TitleName", "TitleName");

                        return View(model);
                    }

                    model.ProfilPhotoPath = uploadedPath;
                }


                if (string.IsNullOrWhiteSpace(password))
                {
                    ModelState.AddModelError("password", "Şifre alanı zorunludur.");

                    ViewData["Titles"] = new SelectList(_context.Titles
                  .Where(t => t.DeletedAt == null && !string.IsNullOrEmpty(t.TitleName))
                  .OrderByDescending(t => t.CreatedAt), "TitleName", "TitleName");

                    return View(model);
                }


                //remove leading and trailing spaces
                model.CreatedAt = DateTime.Now;               
                model.UserName = model.RegistrationNumber?.Trim();
                model.EmailConfirmed = true;
                model.Email = $"{model.RegistrationNumber}@adalet.com"; // Adjust this if needed

                // Create user with provided password
                var result = await _userManager.CreateAsync(model, password);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Personel başarıyla oluşturuldu.";
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.";

                ModelState.AddModelError(string.Empty, "Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz." + ex.Message);
              
            }
                
            ViewData["Titles"] = new SelectList(_context.Titles
                  .Where(t => t.DeletedAt == null && !string.IsNullOrEmpty(t.TitleName))
                  .OrderByDescending(t => t.CreatedAt), "TitleName", "TitleName");

            return View(model);
        }

        //Action method to show edit user form
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID bulunamadı.";
                return RedirectToAction("Error", "Home", new { code = 400 });
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null || user.IsDelete)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Error", "Home", new { code = 404 });
            }


            ViewData["Titles"] = new SelectList(_context.Titles
     .Where(t => t.DeletedAt == null && !string.IsNullOrEmpty(t.TitleName))
     .OrderByDescending(t => t.CreatedAt), "TitleName", "TitleName");


            return View(user);
        }

        //Action method to handle edit user form submission
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser model, string newPassword)
        {
            if (id != model.Id)
            {
                TempData["ErrorMessage"] = "ID bulunamadı.";
                return RedirectToAction("Error", "Home", new { code = 400 });
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewData["Titles"] = new SelectList(_context.Titles
                        .Where(t => t.DeletedAt == null && !string.IsNullOrEmpty(t.TitleName))
                        .OrderByDescending(t => t.CreatedAt), "TitleName", "TitleName");

                    return View(model);
                }

                var user = await _userManager.FindByIdAsync(id);

                if (user == null || user.IsDelete)
                {
                    TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                    return RedirectToAction("Error", "Home", new { code = 404 });
                }


                //check null or empty for registrationnumber
                if (string.IsNullOrEmpty(model.RegistrationNumber))
                {
                    ModelState.AddModelError("RegistrationNumber", "Sicil numarası boş olamaz!");

                    ViewData["Titles"] = new SelectList(_context.Titles
                        .Where(t => t.DeletedAt == null && !string.IsNullOrEmpty(t.TitleName))
                        .OrderByDescending(t => t.CreatedAt), "TitleName", "TitleName");

                    return View(model);
                }


                // If trying to change RegistrationNumber, keep the old value
                if (user.RegistrationNumber != model.RegistrationNumber)
                {
                    ModelState.AddModelError("RegistrationNumber", "Sicil numarası değiştirilemez!");

                    ViewData["Titles"] = new SelectList(_context.Titles
                        .Where(t => t.DeletedAt == null && !string.IsNullOrEmpty(t.TitleName))
                        .OrderByDescending(t => t.CreatedAt), "TitleName", "TitleName");

                    return View(model);
                }

              

                // Check for existing user with the same TCKN (exclude current user)
                if (!string.IsNullOrEmpty(model.TCKN))
                {
                    var tcknOwner = await _userManager.Users
                        .FirstOrDefaultAsync(u => u.TCKN == model.TCKN && u.Id != id && !u.IsDelete);

                    if (tcknOwner != null)
                    {
                        ModelState.AddModelError("TCKN", "Bu TCKN başka bir kullanıcıya ait.");

                        ViewData["Titles"] = new SelectList(_context.Titles
                        .Where(t => t.DeletedAt == null && !string.IsNullOrEmpty(t.TitleName))
                        .OrderByDescending(t => t.CreatedAt), "TitleName", "TitleName");

                        return View(model);
                    }
                }

                // Check for existing user with the same RegistrationNumber (exclude current user)
                if (!string.IsNullOrEmpty(model.RegistrationNumber))
                {
                    var existingUser = await _userManager.Users
                        .FirstOrDefaultAsync(u => u.RegistrationNumber == model.RegistrationNumber && u.Id != id && !u.IsDelete);

                    if (existingUser != null)
                    {
                        ModelState.AddModelError("RegistrationNumber", "Bu sicil numarası başka bir kullanıcıya ait.");

                        ViewData["Titles"] = new SelectList(_context.Titles
                        .Where(t => t.DeletedAt == null && !string.IsNullOrEmpty(t.TitleName))
                        .OrderByDescending(t => t.CreatedAt), "TitleName", "TitleName");


                        return View(model);
                    }
                }


                // Upload profile image using custom extension
                if (model.ProfilPhotoFile != null)
                {
                    var uploadedPath = UploadImageExtension.UploadProfileImage(model.ProfilPhotoFile);

                    if (string.IsNullOrEmpty(uploadedPath))
                    {
                        ModelState.AddModelError("ProfilPhotoFile", "Yüklenen dosya geçersiz veya yüklenemedi.");

                        ViewData["Titles"] = new SelectList(_context.Titles
                        .Where(t => t.DeletedAt == null && !string.IsNullOrEmpty(t.TitleName))
                        .OrderByDescending(t => t.CreatedAt), "TitleName", "TitleName");

                        return View(model);
                    }

                    user.ProfilPhotoPath = uploadedPath;
                }


                // update password if provided
                if (!string.IsNullOrWhiteSpace(newPassword))
                {
                    var hasPassword = await _userManager.HasPasswordAsync(user);

                    if (hasPassword)
                    {
                        var removePassResult = await _userManager.RemovePasswordAsync(user);
                        if (!removePassResult.Succeeded)
                        {
                            foreach (var error in removePassResult.Errors)
                                ModelState.AddModelError(string.Empty, error.Description);

                            ViewData["Titles"] = new SelectList(_context.Titles
                        .Where(t => t.DeletedAt == null && !string.IsNullOrEmpty(t.TitleName))
                        .OrderByDescending(t => t.CreatedAt), "TitleName", "TitleName");

                            return View(model);
                        }
                    }

                    var addPassResult = await _userManager.AddPasswordAsync(user, newPassword);
                    if (!addPassResult.Succeeded)
                    {
                        foreach (var error in addPassResult.Errors)
                            ModelState.AddModelError(string.Empty, error.Description);

                        ViewData["Titles"] = new SelectList(_context.Titles
                        .Where(t => t.DeletedAt == null && !string.IsNullOrEmpty(t.TitleName))
                        .OrderByDescending(t => t.CreatedAt), "TitleName", "TitleName");

                        return View(model);
                    }
                }



                //remove leading and trailing spaces
                user.RegistrationNumber = model.RegistrationNumber?.Trim()?? "";
                user.Title = model.Title;
                user.Name = model.Name;
                user.Surname = model.Surname;
                user.TCKN = model.TCKN;
                user.GSM = model.GSM;
                user.DutyStation = model.DutyStation;

                // If you want to update your Email or Username, it can be done here
                // user.UserName = model.RegistrationNumber;
                // user.Email = $"{model.RegistrationNumber}@domain.com";

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Personel başarıyla güncellendi.";
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.";
                ModelState.AddModelError(string.Empty, "Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz." + ex.Message);
            }

            ViewData["Titles"] = new SelectList(_context.Titles
                        .Where(t => t.DeletedAt == null && !string.IsNullOrEmpty(t.TitleName))
                        .OrderByDescending(t => t.CreatedAt), "TitleName", "TitleName");

            return View(model);
        }



        //Action method to show user details
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID bulunamadı.";
                return RedirectToAction("Error", "Home", new { code = 400 });
            }

            var user = await _userManager.Users
                .OfType<ApplicationUser>()
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDelete);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Error", "Home", new { code = 404 });
            }


            return View(user);
        }


        //Action method to delete a user
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID bulunamadı.";
                return RedirectToAction("Error", "Home", new { code = 400 });
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Error", "Home", new { code = 404 });
            }


            try
            {
                user.IsDelete = true;
                _context.Users.Update(user);

                await _context.SaveChangesAsync();

                return Json(new { Success = "true" });
            }
            catch (Exception ex)
            {

                return Json(new { Success = "false" });
            }

        }

        //Action method to register a new admin
        [Route("adminRegister")]
        [HttpGet]
        public IActionResult AdminRegister()
        {
            // If there is already an admin, registration is disabled
            if (_context.Users.Any(u => u.IsAdmin))
            {              
                return RedirectToAction("adminLogin");
            }

            return View();
        }

        //Action method to handle admin registration form submission
        [Route("adminRegister")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminRegister(AdminRegisterViewModel model)
        {
            if (_context.Users.Any(u => u.IsAdmin))
            {
                TempData["ErrorMessage"] = "Sistemde zaten bir admin kaydı var.";
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                IsAdmin = true,
                EmailConfirmed = true,

                //dummy data for register
                RegistrationNumber = "ADM0001",
                Title = "Bilgi İşlem Müdürü",
                Name = "Admin",
                Surname = "Kullanıcı",
                TCKN = "12345678901", 
                GSM = "5551234567",   
                DutyStation = "Merkez",
                CreatedAt = DateTime.Now,
                IsDelete = false,
                ProfilPhotoPath = null 
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            // If the user is created successfully, assign the Admin role
            if (result.Succeeded)
            {
                //If the Admin role does not exist in the database, create it.
                const string adminRoleName = "Admin";
                if (!await _roleManager.RoleExistsAsync(adminRoleName))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole(adminRoleName));
                    if (!roleResult.Succeeded)
                    {
                        // If the role cannot be created, the user may be deleted or an error may be shown
                        ModelState.AddModelError("", "Admin rolü oluşturulamadı.");
                        return View(model);
                    }
                }

                //Assign Admin role to user
                await _userManager.AddToRoleAsync(user, adminRoleName);
               
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("AdminLogin", "ApplicationUser", new { area = "Admin" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        //Action method to show login form
        [Route("adminLogin")]
        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View();
        }

        //Action method to handle login form submission
        [Route("adminLogin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminLogin(AdminLoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !user.IsAdmin)
            {
                ModelState.AddModelError(string.Empty, "Geçersiz email adresi veya şifre.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                //check admin role
                if (!await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }

                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }


            ModelState.AddModelError(string.Empty, "Geçersiz email adresi veya şifre.");
            return View(model);
        }
              
        //Action method to admin change user password
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AdminChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
         
            if (user == null || user.IsDelete)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Error", "Home", new { code = 404 });
            }


            return View(new ChangePasswordViewModel { UserId = user.Id });
        }

        //Action method to handle password change form submission
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null || user.IsDelete)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Error", "Home", new { code = 404 });
            }


            var changePassResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!changePassResult.Succeeded)
            {
                foreach (var error in changePassResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            await _signInManager.SignOutAsync();

            TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirildi.";

            return RedirectToAction("AdminLogin", "ApplicationUser", new { area = "Admin" });

        }

        //action method to admin forgot password    
        [HttpGet]
        [Route("sifremi-unuttum")]
        public IActionResult AdminForgotPassword()
        {
            return View();
        }

        //action method handle forgot password fprm submission       
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("sifremi-unuttum")]
        public async Task<IActionResult> AdminForgotPassword(AdminForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !user.IsAdmin)
            {
                ModelState.AddModelError(string.Empty, "Sistemde bu e-posta adresiyle kayıtlı bir yönetici bulunamadı.");
                return View(model);
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError(string.Empty, "Bu yönetici e-posta adresi henüz onaylanmamış.");
                return View(model);
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Action(
                "AdminResetPassword",
                "ApplicationUser",
                new { area = "Admin", code = code, email = user.Email },
                protocol: Request.Scheme);

            var mailRequest = new MailRequest
            {
                ToEmail = model.Email,
                Subject = "Yönetici Şifre Sıfırlama",
                Body = $"Şifrenizi sıfırlamak için <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>buraya tıklayın</a>."
            };

            try
            {
                await _mailService.SendEmailAsync(mailRequest);
                TempData["SuccessMessage"] = "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi.";
            }
            catch (Exception ex)
            {
               
                ModelState.AddModelError(string.Empty, "E-posta gönderilirken bir hata oluştu. Lütfen daha sonra tekrar deneyin.");            
            }

            return View(model);
        }

        //action method to reset admin password
        [HttpGet]
        [AllowAnonymous]
        [Route("sifre-sifirla")]
        public IActionResult AdminResetPassword(string code = null, string email = null)
        {
            if (code == null || email == null)
            {
                return BadRequest("Geçersiz şifre sıfırlama bağlantısı.");
            }

            var model = new AdminResetPasswordViewModel
            {
                Code = code,
                Email = email
            };

            return View(model);
        }

        //action method to handle reset password form submission
        [HttpPost]      
        [ValidateAntiForgeryToken]
        [Route("sifre-sifirla")]
        public async Task<IActionResult> AdminResetPassword(AdminResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !user.IsAdmin)
            {          
                return RedirectToAction("AdminResetPasswordConfirmation", "ApplicationUser", new { area = "Admin" });
            }

            var decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));
            var result = await _userManager.ResetPasswordAsync(user, decodedCode, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("AdminResetPasswordConfirmation", "ApplicationUser", new { area = "Admin" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        //action method to show reset password confirmation view
        [HttpGet]       
        [Route("sifre-sifirlandi")]
        public IActionResult AdminResetPasswordConfirmation()
        {
            return View();
        }
    }
}

