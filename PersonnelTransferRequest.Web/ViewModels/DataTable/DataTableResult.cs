namespace PersonnelTransferRequest.Web.ViewModels.DataTable
{
    public class DataTableResult<T>
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public IEnumerable<T> data { get; set; } = new List<T>();
    }
}
