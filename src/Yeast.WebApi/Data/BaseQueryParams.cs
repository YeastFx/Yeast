using Microsoft.AspNetCore.Mvc;

namespace Yeast.WebApi.Data
{
    /// <summary>
    /// Base class form query parameters
    /// </summary>
    public class BaseQueryParams
    {
        /// <summary>
        /// Field name to sort on
        /// Use -filed_name for descending order
        /// </summary>
        [FromQuery(Name = "$sort")]
        public string Sort { get; set; }

        /// <summary>
        /// Start index
        /// </summary>
        [FromQuery(Name = "$offset")]
        public int? Offset { get; set; }

        /// <summary>
        /// Max results to display
        /// </summary>
        [FromQuery(Name = "$limit")]
        public int? Limit { get; set; }

        /// <summary>
        /// Set to true to include total results
        /// </summary>
        [FromQuery(Name = "$count")]
        public bool? Count { get; set; }
    }
}
