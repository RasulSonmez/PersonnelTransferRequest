using PersonnelTransferRequest.Web.ViewModels.DataTable;

namespace PersonnelTransferRequest.Web.Services.DataTable
{
    /// <summary>
    /// Defines a contract for a generic DataTable service that supports server-side processing,
    /// including searching, sorting, and pagination based on a DataTableAjaxPostModel.
    /// </summary>


    public interface IDataTableService
    {
        Task<DataTableResult<T>> GetResultAsync<T>(IQueryable<T> query, DataTableAjaxPostModel model);
    }
}
