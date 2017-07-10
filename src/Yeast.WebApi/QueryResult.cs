using System.Collections;
using System.Collections.Generic;

namespace Yeast.WebApi
{
    /// <summary>
    /// Class to provide query results
    /// </summary>
    public class QueryResult : QueryResultBase
    {
        /// <summary>
        /// Paged query results
        /// </summary>
        public IEnumerable Data { get; set; }
    }

    /// <summary>
    /// Class to provide query results
    /// </summary>
    /// <typeparam name="T">Queried object type</typeparam>
    public class QueryResult<T> : QueryResultBase
    {
        /// <summary>
        /// Paged query results
        /// </summary>
        public IEnumerable<T> Data { get; set; }
    }
}
