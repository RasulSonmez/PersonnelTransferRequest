using PersonnelTransferRequest.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelTransferRequest.Entities.Models
{
    public class TransferRequest : BaseEntity
    {
        
     
        [MinLength(3, ErrorMessage = "Açıklama en az 3 karakter olmalıdır.")]
        [MaxLength(5000, ErrorMessage = "Açıklama en fazla 5000 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Display(Name = "Talep Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Display(Name = "Talep Nedeni")]
        public RequestType RequestType { get; set; }

        [Display(Name = "Talep Durumu")]
        public TransferStatus Status { get; set; } = TransferStatus.Pending;

        public ICollection<TransferPreference>? Preferences { get; set; } = new List<TransferPreference>();

        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

    }
}
