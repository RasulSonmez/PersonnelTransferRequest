namespace PersonnelTransferRequest.Web.Areas.Admin.ViewModel
{
    public class SupportMessageDataTableViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
