using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelTransferRequest.Entities.Enums
{
    public enum RequestType
    {
        [Display(Name = "Genel Tayin")]
        General = 0,

        [Display(Name = "Sağlık")]
        Health = 1,

        [Display(Name = "Aile")]
        Family = 2,

        [Display(Name = "Eğitim")]
        Education = 3,

        [Display(Name = "Diğer Sebepler")]
        Other = 4
    }
}
