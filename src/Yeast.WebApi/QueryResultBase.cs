using Newtonsoft.Json;

namespace Yeast.WebApi
{
    /// <summary>
    /// QueryResult base class
    /// </summary>
    public abstract class QueryResultBase
    {
        /// <summary>
        /// Total number of results
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? TotalResults { get; set; }
    }
}
