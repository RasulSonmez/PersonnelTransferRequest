using System.ComponentModel.DataAnnotations;

namespace PersonnelTransferRequest.Web.Areas.Admin.ViewModel
{
    public class AdminLoginViewModel
    {
        [Required(ErrorMessage = "Email zorunludur.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}
