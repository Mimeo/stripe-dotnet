namespace Stripe
{
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// A Stripe client, used to issue requests to Stripe's API and deserialize responses.
    /// </summary>
    public class StripeClient : IStripeClient
    {
        private readonly string apiBase;

        private readonly string apiKey;

        private readonly string clientId;

        private readonly string connectBase;

        private readonly string filesBase;

        /// <summary>Initializes a new instance of the <see cref="StripeClient"/> class.</summary>
        /// <param name="apiBase">TODO apiBase.</param>
        /// <param name="apiKey">The API key to use to authenticate requests with Stripe.</param>
        /// <param name="clientId">The client ID to use in OAuth requests.</param>
        /// <param name="connectBase">TODO connectBase.</param>
        /// <param name="filesBase">TODO filesBase.</param>
        /// <param name="httpClient">
        /// The <see cref="IHttpClient"/> client to use. If <c>null</c>, an HTTP client will be
        /// created with default parameters.
        /// </param>
        public StripeClient(
            string apiBase = null,
            string apiKey = null,
            string clientId = null,
            string connectBase = null,
            string filesBase = null,
            IHttpClient httpClient = null)
        {
            this.apiBase = apiBase;
            this.apiKey = apiKey;
            this.clientId = clientId;
            this.connectBase = connectBase;
            this.filesBase = filesBase;
            this.HttpClient = httpClient ?? BuildDefaultHttpClient();
        }

        public string ApiBase
        {
            get => this.apiBase ?? StripeConfiguration.ApiBase;
        }

        /// <summary>Gets the API key used to authenticate requests with Stripe.</summary>
        /// <value>The API key used to authenticate requests with Stripe.</value>
        public string ApiKey
        {
            get => this.apiKey ?? StripeConfiguration.ApiKey;
        }

        /// <summary>Gets the client ID to use in OAuth requests.</summary>
        /// <value>The client ID to use in OAuth requests.</value>
        public string ClientId
        {
            get => this.clientId ?? StripeConfiguration.ClientId;
        }

        public string ConnectBase
        {
            get => this.connectBase ?? StripeConfiguration.ConnectBase;
        }

        public string FilesBase
        {
            get => this.filesBase ?? StripeConfiguration.FilesBase;
        }

        /// <summary>Gets the <see cref="IHttpClient"/> used to send HTTP requests.</summary>
        /// <value>The <see cref="IHttpClient"/> used to send HTTP requests.</value>
        public IHttpClient HttpClient { get; }

        /// <summary>Sends a request to Stripe's API as an asynchronous operation.</summary>
        /// <typeparam name="T">Type of the Stripe entity returned by the API.</typeparam>
        /// <param name="method">The HTTP method.</param>
        /// <param name="path">The path of the request.</param>
        /// <param name="options">The parameters of the request.</param>
        /// <param name="requestOptions">The special modifiers of the request.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="StripeException">Thrown if the request fails.</exception>
        public async Task<T> RequestAsync<T>(
            HttpMethod method,
            string path,
            BaseOptions options,
            RequestOptions requestOptions,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : IStripeEntity
        {
            var request = new StripeRequest(this, method, path, options, requestOptions);

            var response = await this.HttpClient.MakeRequestAsync(request, cancellationToken);

            return ProcessResponse<T>(response);
        }

        private static IHttpClient BuildDefaultHttpClient()
        {
            return new SystemNetHttpClient();
        }

        private static T ProcessResponse<T>(StripeResponse response)
            where T : IStripeEntity
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw BuildStripeException(response);
            }

            T obj;
            try
            {
                obj = StripeEntity.FromJson<T>(response.Content);
            }
            catch (Newtonsoft.Json.JsonException)
            {
                throw BuildInvalidResponseException(response);
            }

            obj.StripeResponse = response;

            return obj;
        }

        private static StripeException BuildStripeException(StripeResponse response)
        {
            JObject jObject = null;

            try
            {
                jObject = JObject.Parse(response.Content);
            }
            catch (Newtonsoft.Json.JsonException)
            {
                return BuildInvalidResponseException(response);
            }

            // If the value of the `error` key is a string, then the error is an OAuth error
            // and we instantiate the StripeError object with the entire JSON.
            // Otherwise, it's a regular API error and we instantiate the StripeError object
            // with just the nested hash contained in the `error` key.
            var errorToken = jObject["error"];
            if (errorToken == null)
            {
                return BuildInvalidResponseException(response);
            }

            var stripeError = errorToken.Type == JTokenType.String
                ? StripeError.FromJson(response.Content)
                : StripeError.FromJson(errorToken.ToString());

            stripeError.StripeResponse = response;

            return new StripeException(
                response.StatusCode,
                stripeError,
                stripeError.Message ?? stripeError.ErrorDescription)
            {
                StripeResponse = response,
            };
        }

        private static StripeException BuildInvalidResponseException(StripeResponse response)
        {
            return new StripeException(
                response.StatusCode,
                null,
                $"Invalid response object from API: \"{response.Content}\"")
            {
                StripeResponse = response,
            };
        }
    }
}
