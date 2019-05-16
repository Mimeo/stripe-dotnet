namespace Stripe
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ReturnListOptions : ListOptionsWithCreated
    {
        /// <summary>
        /// The order to retrieve returns for.
        /// </summary>
        [JsonProperty("order")]
        public List<string> Ids { get; set; }
    }
}
