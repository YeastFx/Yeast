using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Yeast.WebApi
{
    /// <summary>
    /// Base class for API request parameters
    /// </summary>
    public class ApiRequestParams
    {
        /// <summary>
        /// List of navigation properties to include
        /// </summary>
        [FromQuery(Name = "$include")]
        public string[] Include { get; set; }
    }
}
