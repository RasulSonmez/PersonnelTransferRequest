using System.ComponentModel.DataAnnotations;

namespace PersonnelTransferRequest.Web.Areas.Admin.ViewModel
{
    public class ChangePasswordViewModel
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "Mevcut şifre zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mevcut Şifre")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Yeni şifre zorunludur.")]
        [StringLength(100, ErrorMessage = "Şifre en az {2} karakter olmalı.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Şifre Tekrar zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")]
        [Compare("NewPassword", ErrorMessage = "Şifreler uyuşmuyor.")]
        public string ConfirmPassword { get; set; }
    }
}
