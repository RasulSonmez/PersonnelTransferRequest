using PersonnelTransferRequest.Web.ViewModels.DataTable;

namespace PersonnelTransferRequest.Web.Services.DataTable
{
    public interface IDataTableService
    {
        Task<DataTableResult<T>> GetResultAsync<T>(IQueryable<T> query, DataTableAjaxPostModel model);
    }
}
