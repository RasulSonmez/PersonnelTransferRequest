using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelTransferRequest.Entities.Models
{
    public class Title:BaseEntity
    {
        [Required(ErrorMessage = "Unvan alanı zorunludur.")]
        [MinLength(3, ErrorMessage = "Unvan en az 3 karakter olmalıdır.")]
        [MaxLength(50, ErrorMessage = "Unvan en fazla 50 karakter olabilir.")]
        [Display(Name = "Unvan Adı")]
        public string TitleName { get; set; }
    }
}
