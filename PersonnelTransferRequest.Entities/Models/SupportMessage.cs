using PersonnelTransferRequest.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelTransferRequest.Entities.Models
{
    public class SupportMessage : BaseEntity
    {
        [Display(Name = "Başlık")]
        [Required(ErrorMessage = "Zorunlu alan.")]
        [MaxLength(256, ErrorMessage = "256 karakterden uzun olamaz")]
        [MinLength(2, ErrorMessage = "2 karakterden kısa olamaz")]
        public string Title { get; set; }

        [Display(Name = "Mesaj")]
        [Required(ErrorMessage = "Zorunlu alan")]
        [MaxLength(5000, ErrorMessage = "5000 karakterden uzun olamaz")]
        [MinLength(2, ErrorMessage = "2 karakterden kısa olamaz")]
        public string? Message { get; set; }
        public SupportStatus? Status { get; set; }
        public List<SupportMessageReply> SupportMessageReplies { get; set; } = new List<SupportMessageReply>();
    }
}
