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
        public string Sort { get; set; }

        /// <summary>
        /// Start index
        /// </summary>
        public int? Offset { get; set; }

        /// <summary>
        /// Max results to display
        /// </summary>
        public int? Limit { get; set; }
    }
}
