using System.ComponentModel.DataAnnotations;

namespace PersonnelTransferRequest.Web.Areas.Admin.ViewModel
{
    public class AdminForgotPasswordViewModel
    {
        [Required(ErrorMessage = "E-posta adresi gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; }
    }

}
