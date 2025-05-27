using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelTransferRequest.Common.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string orderByMember, bool ascending)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var prop = Expression.PropertyOrField(param, orderByMember);
            var sort = Expression.Lambda(prop, param);
            var call = Expression.Call(
                typeof(Queryable),
                ascending ? "OrderBy" : "OrderByDescending",
                new[] { typeof(T), prop.Type },
                query.Expression,
                Expression.Quote(sort));
            return query.Provider.CreateQuery<T>(call);
        }
    }
}
