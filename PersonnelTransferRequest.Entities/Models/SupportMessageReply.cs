using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelTransferRequest.Entities.Models
{
    public class SupportMessageReply : BaseEntity
    {
        [Display(Name = "Mesaj")]
        [Required(ErrorMessage = "Zorunlu alan.")]
        [MaxLength(5000, ErrorMessage = "5000 karakterden uzun olamaz")]
        [MinLength(2, ErrorMessage = "2 karakterden kısa olamaz")]
        public string? Message { get; set; }
        public int? SupportMessageId { get; set; }
        public SupportMessage? SupportMessage { get; set; }
    }
}
