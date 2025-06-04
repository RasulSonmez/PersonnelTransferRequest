namespace PersonnelTransferRequest.Web.Areas.Admin.ViewModel
{
    public class TransferRequestDataTableViewModel
    {
        public int Id { get; set; }
        public DateTime RequestDate { get; set; }
        public string RegistrationNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Gsm { get; set; } = string.Empty;
    }

}
