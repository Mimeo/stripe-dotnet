namespace StripeTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Stripe;
    using Xunit;

    public class ReturnServiceTest : BaseStripeTest
    {
        private const string ReturnId = "orret_123";

        private readonly ReturnService service;
        private readonly ReturnListOptions listOptions;

        public ReturnServiceTest(MockHttpClientFixture mockHttpClientFixture)
            : base(mockHttpClientFixture)
        {
            this.service = new ReturnService();

            this.listOptions = new ReturnListOptions
            {
                Limit = 1,
            };
        }

        [Fact]
        public void Get()
        {
            var orderReturn = this.service.Get(ReturnId);
            this.AssertRequest(HttpMethod.Get, $"/v1/order_returns/{ReturnId}");
            Assert.NotNull(orderReturn);
            Assert.Equal("order_return", orderReturn.Object);
        }

        [Fact]
        public async Task GetAsync()
        {
            var orderReturn = await this.service.GetAsync(ReturnId);
            this.AssertRequest(HttpMethod.Get, $"/v1/order_returns/{ReturnId}");
            Assert.NotNull(orderReturn);
            Assert.Equal("order_return", orderReturn.Object);
        }

        [Fact]
        public void List()
        {
            var orderReturns = this.service.List(this.listOptions);
            this.AssertRequest(HttpMethod.Get, "/v1/order_returns");
            Assert.NotNull(orderReturns);
            Assert.Equal("list", orderReturns.Object);
            Assert.Single(orderReturns.Data);
            Assert.Equal("order_return", orderReturns.Data[0].Object);
        }

        [Fact]
        public async Task ListAsync()
        {
            var orderReturns = await this.service.ListAsync(this.listOptions);
            this.AssertRequest(HttpMethod.Get, "/v1/order_returns");
            Assert.NotNull(orderReturns);
            Assert.Equal("list", orderReturns.Object);
            Assert.Single(orderReturns.Data);
            Assert.Equal("order_return", orderReturns.Data[0].Object);
        }

        [Fact]
        public void ListAutoPaging()
        {
            var orderReturns = this.service.ListAutoPaging(this.listOptions).ToList();
            Assert.NotNull(orderReturns);
            Assert.Equal("order_return", orderReturns[0].Object);
        }
    }
}
