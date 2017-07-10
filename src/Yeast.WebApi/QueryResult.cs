using Newtonsoft.Json;
using System.Collections;

namespace Yeast.WebApi
{
    /// <summary>
    /// Class to provide query results
    /// </summary>
    public class QueryResult
    {
        /// <summary>
        /// Paged query results
        /// </summary>
        public IEnumerable Data { get; set; }

        /// <summary>
        /// Total number of results
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public virtual int? TotalResults { get; set; }
    }
}
