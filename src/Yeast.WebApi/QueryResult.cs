using Newtonsoft.Json;
using System.Collections.Generic;

namespace Yeast.WebApi
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
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? TotalResults { get; set; }
    }
}
