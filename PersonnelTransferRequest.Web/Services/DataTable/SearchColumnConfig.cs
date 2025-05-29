using PersonnelTransferRequest.Entities.Models;

namespace PersonnelTransferRequest.Web.Services.DataTable
{

    /// <summary>
    /// Provides a centralized configuration for specifying which string properties
    /// of each entity type should be included in dynamic search operations (e.g., DataTable filtering).
    /// This allows generic services to retrieve searchable column names based on entity type,
    /// enabling consistent and maintainable dynamic search logic.
    /// </summary>


    public static class SearchColumnConfig
    {
        private static readonly Dictionary<Type, string[]> _searchableColumns = new()
        {
            { typeof(ApplicationUser), new[] { "Name", "Surname", "RegistrationNumber", "UserName", "Email", "GSM" } },
            { typeof(Title), new[] { "TitleName" } },
            //Other entities can be added here
        };

        public static string[] GetSearchableColumnsFor<T>()
        {
            return _searchableColumns.TryGetValue(typeof(T), out var columns)
                ? columns
                : Array.Empty<string>();
        }
    }
}
