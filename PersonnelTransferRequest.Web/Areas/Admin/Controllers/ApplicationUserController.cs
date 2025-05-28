using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PersonnelTransferRequest.Common.Extensions;
using PersonnelTransferRequest.Entities.Models;
using PersonnelTransferRequest.Web.Data;
using PersonnelTransferRequest.Web.Services.DataTable;
using PersonnelTransferRequest.Web.ViewModels.DataTable;

namespace PersonnelTransferRequest.Web.Areas.Admin.Controllers
{
   
    public class ApplicationUserController : AdminBaseController
    {
     

       public ApplicationUserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IDataTableService dataTableService)
        :  base(context, userManager, dataTableService)
        {
        }

        //Action method to list all users
        [HttpGet]
        public IActionResult Index()
        {
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
                return StatusCode(500, "Something went wrong: " + ex.Message);
            }
        }

        //Action method to show create user form
        public IActionResult Create()
        {          
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
                    return View(model);

                // Check for existing user with the same TCKN
                if (!string.IsNullOrEmpty(model.TCKN))
                {
                    var existingUser = await _userManager.Users
                        .FirstOrDefaultAsync(u => u.TCKN == model.TCKN && !u.IsDelete);

                    if (existingUser != null)
                    {
                        ModelState.AddModelError("TCKN", "TCKN doğru olduğuna emin olunuz.");
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
                        return View(model);
                    }

                    model.ProfilPhotoPath = uploadedPath;
                }

                //remove leading and trailing spaces
                model.UserName = model.RegistrationNumber?.Trim();
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

            return View(model);
        }

        //Action method to show edit user form
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null || user.IsDelete)
                return NotFound();

            return View(user);
        }

        //Action method to handle edit user form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser model)
        {
            if (id != model.Id)
                return NotFound();

            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var user = await _userManager.FindByIdAsync(id);
                if (user == null || user.IsDelete)
                    return NotFound();

                // Check for existing user with the same TCKN
                if (!string.IsNullOrEmpty(model.TCKN))
                {
                    var tcknOwner = await _userManager.Users
                        .FirstOrDefaultAsync(u => u.TCKN == model.TCKN && u.Id != id && !u.IsDelete);

                    if (tcknOwner != null)
                    {
                        ModelState.AddModelError("TCKN", "TCKN doğru olduğuna emin olunuz.");
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
                        return View(model);
                    }

                    user.ProfilPhotoPath = uploadedPath;
                }

                //remove leading and trailing spaces
                user.RegistrationNumber = model.RegistrationNumber?.Trim();
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

            return View(model);
        }



        //Action method to show user details
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.Users
                .OfType<ApplicationUser>()
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDelete);

            if (user == null)
            {
                return NotFound();
            }
        
            return View(user);
        }


        //Action method to delete a user
        [HttpGet]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
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
    }
}
