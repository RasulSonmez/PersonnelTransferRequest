using System.ComponentModel.DataAnnotations;

namespace PersonnelTransferRequest.Web.Areas.Admin.ViewModel
{
    public class AdminResetPasswordViewModel
    {
        [Required(ErrorMessage = "Email alanı zorunludur.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [StringLength(100, ErrorMessage = "Şifre en az {2}, en fazla {1} karakter olmalı.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "Şifre tekrar alanı zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre (Tekrar)")]
        [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;
    }

}
