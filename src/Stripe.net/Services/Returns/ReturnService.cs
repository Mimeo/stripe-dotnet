namespace Stripe
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class ReturnService : Service<OrderReturn>,
        IListable<OrderReturn, ReturnListOptions>,
        IRetrievable<OrderReturn>
    {
        public ReturnService()
            : base(null)
        {
        }

        public ReturnService(string apiKey)
            : base(apiKey)
        {
        }

        public override string BasePath => "/order_returns";

        public bool ExpandCharge { get; set; }

        public bool ExpandCustomer { get; set; }

        public virtual OrderReturn Get(string returnId, RequestOptions requestOptions = null)
        {
            return this.GetEntity(returnId, null, requestOptions);
        }

        public virtual Task<OrderReturn> GetAsync(string returnId, RequestOptions requestOptions = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.GetEntityAsync(returnId, null, requestOptions, cancellationToken);
        }

        public virtual StripeList<OrderReturn> List(ReturnListOptions options = null, RequestOptions requestOptions = null)
        {
            return this.ListEntities(options, requestOptions);
        }

        public virtual Task<StripeList<OrderReturn>> ListAsync(ReturnListOptions options = null, RequestOptions requestOptions = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.ListEntitiesAsync(options, requestOptions, cancellationToken);
        }

        public virtual IEnumerable<OrderReturn> ListAutoPaging(ReturnListOptions options = null, RequestOptions requestOptions = null)
        {
            return this.ListEntitiesAutoPaging(options, requestOptions);
        }
    }
}
