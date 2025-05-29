using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelTransferRequest.Entities.Enums
{
    public enum TransferStatus
    {
        [Display(Name = "Beklemede")]
        Pending = 0,

        [Display(Name = "Onaylandı")]
        Approved = 1,

        [Display(Name = "Reddedildi")]
        Rejected = 2
    }
}
