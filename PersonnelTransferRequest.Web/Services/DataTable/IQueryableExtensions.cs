using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelTransferRequest.Common.Extensions
{
    /// <summary>
 /// Dynamically builds an OR-based 'Where' clause using 'Contains' for string properties.
 /// Filters the IQueryable<T> source based on whether any of the specified string properties
 /// contain the given value.
 /// </summary>
 /// <typeparam name="T">Type of the elements in the source IQueryable.</typeparam>
 /// <param name="source">The source IQueryable to filter.</param>
 /// <param name="propertyNames">Array of property names (must be strings) to apply the Contains filter.</param>
 /// <param name="value">The value to search for within the specified properties.</param>
 /// <returns>A filtered IQueryable<T> where any of the specified string properties contain the value.</returns>
 /// 

    public static class IQueryableExtensions
    {

        public static IQueryable<T> WhereDynamicOrContains<T>(this IQueryable<T> source, string[] propertyNames, string value)
        {
            var parameter = Expression.Parameter(typeof(T), "p");
            Expression? orExpression = null;

            foreach (var propName in propertyNames)
            {
                var property = Expression.PropertyOrField(parameter, propName);

                if (property.Type != typeof(string))
                    continue;

                var method = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
                var constant = Expression.Constant(value, typeof(string));
                var containsCall = Expression.Call(property, method, constant);

                orExpression = orExpression == null
                    ? containsCall
                    : Expression.OrElse(orExpression, containsCall);
            }

            if (orExpression == null)
                return source;

            var lambda = Expression.Lambda<Func<T, bool>>(orExpression, parameter);
            return source.Where(lambda);
        }



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
