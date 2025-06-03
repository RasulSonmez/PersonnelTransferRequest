using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelTransferRequest.Entities.Enums
{
    public enum SupportStatus : byte
    {
        [Display(Name = "Yeni")]
        New = 0,
        [Display(Name = "Cevap verildi")]
        InProcess = 1,       
    }
}
