using PersonnelTransferRequest.Common.Extensions;
using PersonnelTransferRequest.Web.ViewModels.DataTable;

namespace PersonnelTransferRequest.Web.Services.DataTable
{
    public class DataTableService : IDataTableService
    {
        public async Task<DataTableResult<T>> GetResultAsync<T>(IQueryable<T> query, DataTableAjaxPostModel model)
        {
            var result = new DataTableResult<T>
            {
                draw = model.draw,
                recordsTotal = await Task.FromResult(query.Count())
            };

            // Search
            if (!string.IsNullOrEmpty(model.search?.value))
            {
                var searchValue = model.search.value;
                var searchableColumns = new[] { "Name", "Surname", "RegistrationNumber", "UserName" };

                query = query.WhereDynamicOrContains(searchableColumns, searchValue);
            }

            result.recordsFiltered = await Task.FromResult(query.Count());

            // Order
            if (model.order != null && model.order.Any())
            {
                var sortColumn = model.columns[model.order[0].column].data;
                var sortAsc = model.order[0].dir == "asc";

                query = query.OrderByDynamic(sortColumn, sortAsc);
            }

            // Pagination
            query = query.Skip(model.start).Take(model.length);

            result.data = await Task.FromResult(query.ToList());

            return result;
        }
    }
}
