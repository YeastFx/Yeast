using System.Collections.Generic;

namespace Yeast.WebApi.Data
{
    /// <summary>
    /// Class to provide query results
    /// </summary>
    /// <typeparam name="T">Queried object type</typeparam>
    public class QueryResult<T>
    {
        /// <summary>
        /// Paged query results
        /// </summary>
        public IEnumerable<T> Data { get; set; }

        /// <summary>
        /// Total number of results
        /// </summary>
        public int? TotalResults { get; set; }
    }
}
