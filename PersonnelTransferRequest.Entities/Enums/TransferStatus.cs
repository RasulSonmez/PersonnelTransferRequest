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
        Pending = 1,

        [Display(Name = "Onaylandı")]
        Approved = 2,

        [Display(Name = "Reddedildi")]
        Rejected = 3
    }
}
