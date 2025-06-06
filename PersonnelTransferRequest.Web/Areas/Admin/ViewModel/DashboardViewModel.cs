﻿using PersonnelTransferRequest.Entities.Models;

namespace PersonnelTransferRequest.Web.Areas.Admin.ViewModel
{
    public class DashboardViewModel
    {
        public int PersonnelCount { get; set; }
        public int TransferRequestCount { get; set; }
        public int TitleCount { get; set; }
        public int SupportMessageCount { get; set; }

        public List<TransferRequest>? TransferRequests { get; set; }
        public List<SupportMessage>? SupportMessages { get; set; }
    }
}
