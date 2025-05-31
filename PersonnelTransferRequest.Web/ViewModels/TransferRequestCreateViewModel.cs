using PersonnelTransferRequest.Entities.Enums;
using PersonnelTransferRequest.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace PersonnelTransferRequest.Web.ViewModels
{
    public class TransferRequestCreateViewModel
    {
        [MinLength(3, ErrorMessage = "Açıklama en az 3 karakter olmalıdır.")]
        [MaxLength(5000, ErrorMessage = "Açıklama en fazla 5000 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Zorunlu alan")]
        public RequestType RequestType { get; set; }

        [MaxLength(5, ErrorMessage = "En fazla 5 tercih yapabilirsiniz.")]
        public List<TransferPreferenceViewModel> Preferences { get; set; } = new List<TransferPreferenceViewModel>();
    }

    public class TransferPreferenceViewModel
    {

        [Required(ErrorMessage = "Lütfen bir adalet sarayı tercih ediniz.")]
        [Display(Name = "Tercih Edilen Adalet Sarayı")]
        public string CourtHouse { get; set; }

        [Range(1, 5, ErrorMessage = "Tercih sırası 1 ile 5 arasında olmalıdır.")]
        public int PriorityOrder { get; set; }
    }

}
