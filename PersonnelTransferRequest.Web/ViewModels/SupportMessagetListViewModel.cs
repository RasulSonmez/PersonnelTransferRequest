using PersonnelTransferRequest.Entities.Models;

namespace PersonnelTransferRequest.Web.ViewModels
{
    public class SupportMessagetListViewModel
    {
        public List<SupportMessage> SupportMessages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
