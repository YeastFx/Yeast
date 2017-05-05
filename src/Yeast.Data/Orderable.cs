using System;
using System.Linq;
using System.Linq.Expressions;

namespace Yeast.Data
{
    /// <summary>
    /// Helper class to sort a query
    /// </summary>
    /// <typeparam name="T">Queried type</typeparam>
    public class Orderable<T>
    {
        private IQueryable<T> _queryable;

        /// <summary>
        /// Creates a new orderable instance
        /// </summary>
        /// <param name="queryable">The IQueryable to sort</param>
        public Orderable(IQueryable<T> queryable)
        {
            _queryable = queryable;
        }

        /// <summary>
        /// Soerted IQueryable instance
        /// </summary>
        public IQueryable<T> Queryable {
            get { return _queryable; }
        }

        /// <summary>
        /// Sorts ascending on specified key
        /// </summary>
        /// <typeparam name="TKey">Sort key type</typeparam>
        /// <param name="keySelector">Sort key selector</param>
        /// <returns></returns>
        public Orderable<T> Asc<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            _queryable = _queryable
                .OrderBy(keySelector);
            return this;
        }

        /// <summary>
        /// Sorts descending on specified key
        /// </summary>
        /// <typeparam name="TKey">Sort key type</typeparam>
        /// <param name="keySelector">Sort key selector</param>
        /// <returns></returns>
        public Orderable<T> Desc<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            _queryable = _queryable
                .OrderByDescending(keySelector);
            return this;
        }

        /// <summary>
        /// Sorts on specified key
        /// </summary>
        /// <typeparam name="TKey">Sort key type</typeparam>
        /// <param name="order">Sort order</param>
        /// <param name="keySelector">Sort key selector</param>
        /// <returns></returns>
        public Orderable<T> Sort<TKey>(SortOrder order, Expression<Func<T, TKey>> keySelector)
        {
            if (order == SortOrder.desc)
            {
                return Desc(keySelector);
            }
            else
            {
                return Asc(keySelector);
            }
        }
    }

    /// <summary>
    /// Oderable sorting order
    /// </summary>
    public enum SortOrder
    {
        /// <summary>
        /// Ascending sort order
        /// </summary>
        asc,
        /// <summary>
        /// Descending sort order
        /// </summary>
        desc
    }
}
