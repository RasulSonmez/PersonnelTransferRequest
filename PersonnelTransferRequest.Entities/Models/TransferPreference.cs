using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelTransferRequest.Entities.Models
{
    public class TransferPreference : BaseEntity
    {


        [Required(ErrorMessage = "Lütfen bir adalet sarayı tercih ediniz")]
        [Display(Name = "Tercih Edilen Adalet Sarayı")]
        public string CourtHouse { get; set; }

        [Range(1, 5)]
        [Display(Name = "Tercih Sırası")]
        public int PriorityOrder { get; set; }

        [Required(ErrorMessage = "Tayin Talebi alanı zorunludur.")]
        public int TransferRequestId { get; set; }
        public TransferRequest TransferRequest { get; set; }
    }

}
