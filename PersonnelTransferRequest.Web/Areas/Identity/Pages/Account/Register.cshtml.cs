﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonnelTransferRequest.Common.Extensions;
using PersonnelTransferRequest.Entities.Models;
using PersonnelTransferRequest.Web.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

namespace PersonnelTransferRequest.Web.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            ApplicationDbContext context,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// 

        public class InputModel
        {
            [Required(ErrorMessage = "Email alanı zorunludur.")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Şifre alanı zorunludur.")]
            [StringLength(100, ErrorMessage = "Şifre en az {2}, en fazla {1} karakter olmalı.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Şifre")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Şifre (Tekrar)")]
            [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Zorunlu alan")]
            [Display(Name = "Sicil")]
            [MinLength(1), MaxLength(10)]
            public string RegistrationNumber { get; set; }

            [Required(ErrorMessage = "Zorunlu alan")]
            [Display(Name = "Unvan")]
            public string Title { get; set; }

            [Required(ErrorMessage = "Zorunlu alan")]
            [MinLength(3), MaxLength(128)]
            [Display(Name = "İsim")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Zorunlu alan")]
            [MinLength(3), MaxLength(200)]
            [Display(Name = "Soyisim")]
            public string Surname { get; set; }

            [Required(ErrorMessage = "Zorunlu alan")]
            [RegularExpression(@"^[0-9]{11}$", ErrorMessage = "Hatalı TC Kimlik No formatı!")]
            [Display(Name = "TC Kimlik No")]
            public string TCKN { get; set; }

            [Required(ErrorMessage = "Zorunlu alan")]
            [RegularExpression(@"^\d{10,15}$", ErrorMessage = "Sadece rakam girin. 10-15 haneli olmalı.")]
            [Display(Name = "Telefon Numarası")]
            public string GSM { get; set; }

            [Required(ErrorMessage = "Zorunlu alan")]
            [Display(Name = "Görev Yeri")]
            public string DutyStation { get; set; }

            [Display(Name = "Oluşturulma Tarihi")]
            [DataType(DataType.DateTime)]
            public DateTime? CreatedAt { get; set; } = DateTime.Now;

            [Display(Name = "Fotoğraf")]
            [DataType(DataType.Upload)]
            [MaxFileSize(5 * 1024 * 1024)] // 5 MB sınır
            [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg", ".svg" })]
            public IFormFile? ProfilPhotoFile { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {

            ReturnUrl = returnUrl;

            ViewData["Titles"] = new SelectList(_context.Titles
      .Where(t => t.DeletedAt == null && !string.IsNullOrEmpty(t.TitleName))
      .OrderByDescending(t => t.CreatedAt), "TitleName", "TitleName");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var existingTCKN = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.TCKN == Input.TCKN);

                if (existingTCKN != null)
                {
                    ModelState.AddModelError("Input.TCKN", "TCKN doğru olduğuna emin olunuz.");
                }
                else
                {
                    var user = CreateUser();

                    await _userStore.SetUserNameAsync(user, Input.RegistrationNumber, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                    user.Title = Input.Title;
                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Yeni kullanıcı başarıyla oluşturuldu.");
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        TempData["SuccessMessage"] = "Kullanıcı başarıyla oluşturuldu.";
                        return LocalRedirect(returnUrl);
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            ViewData["Titles"] = new SelectList(_context.Titles
     .Where(t => t.DeletedAt == null && !string.IsNullOrEmpty(t.TitleName))
     .OrderByDescending(t => t.CreatedAt), "TitleName", "TitleName", Input.Title);


            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                var user = new ApplicationUser
                {
                    CreatedAt = Input.CreatedAt ?? DateTime.Now,
                    EmailConfirmed = true,
                    RegistrationNumber = Input.RegistrationNumber,
                    Title = Input.Title,
                    Name = Input.Name,
                    Surname = Input.Surname,
                    TCKN = Input.TCKN,
                    GSM = Input.GSM,
                    DutyStation = Input.DutyStation,
                    UserName = Input.RegistrationNumber,
                    Email = Input.Email,
                    IsDelete = false
                };


                // Upload profile image using custom extension
                if (Input.ProfilPhotoFile != null)
                {
                    var uploadedPath = UploadImageExtension.UploadProfileImage(Input.ProfilPhotoFile);

                    if (!string.IsNullOrEmpty(uploadedPath))
                    {
                        user.ProfilPhotoPath = uploadedPath;
                    }
                }

                return user;
            }
            catch
            {
                throw new InvalidOperationException($"'{nameof(ApplicationUser)}' örneği oluşturulamadı.");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("Varsayılan kullanıcı arayüzü e-posta desteği olan bir kullanıcı deposu gerektirir.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
