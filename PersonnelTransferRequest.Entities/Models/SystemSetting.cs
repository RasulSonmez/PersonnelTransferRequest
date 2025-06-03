using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelTransferRequest.Entities.Models
{
    public class SystemSetting
    {
        public int Id { get; set; }
        public bool IsTransferRequestOpen { get; set; }
    }
}
