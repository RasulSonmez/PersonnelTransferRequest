using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PersonnelTransferRequest.Common.Extensions;
using PersonnelTransferRequest.Entities.Models;
using PersonnelTransferRequest.Web.Areas.Admin.ViewModel;
using PersonnelTransferRequest.Web.Data;
using PersonnelTransferRequest.Web.Services.DataTable;
using PersonnelTransferRequest.Web.ViewModels.DataTable;

namespace PersonnelTransferRequest.Web.Areas.Admin.Controllers
{
   
    public class ApplicationUserController : AdminBaseController
    {
     

       public ApplicationUserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IDataTableService dataTableService, SignInManager<ApplicationUser> signInManager)
        :  base(context, userManager, dataTableService, signInManager)
        {
        }

        //Action method to list all users
        [HttpGet]
        public IActionResult Index()
        {
            //data comes from dynamic data table GetAllTitlesForDataTable
            return View();
        }

        //Action method to load user data for DataTable
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetAllUserForDataTable(DataTableAjaxPostModel model)
        {
            try
            {
                var query = _context.Users.Where(u => !u.IsDelete);
                var result = await _dataTableService.GetResultAsync(query, model);
                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Birşeyler ters gitti: " + ex.Message);
            }
        }

        //Action method to show create user form
        public IActionResult Create()
        {
            ViewData["Titles"] = new SelectList(_context.Titles
    .Where(t => t.DeletedAt == null && !string.IsNullOrEmpty(t.TitleName))
    .OrderByDescending(t => t.CreatedAt), "TitleName", "TitleName");

            return View();
        }


        //Action method to show create user form
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


        //Action method to change user password
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

            return RedirectToAction("Login", "Account", new { area = "Identity" });

        }

    }
}

