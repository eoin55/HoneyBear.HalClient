using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using HoneyBear.HalClient.Http;
using HoneyBear.HalClient.Models;
using HoneyBear.HalClient.Unit.Tests.ProxyResources;
using Newtonsoft.Json;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoRhinoMock;
using Ploeh.SemanticComparison.Fluent;
using Rhino.Mocks;
using Version = HoneyBear.HalClient.Unit.Tests.ProxyResources.Version;

namespace HoneyBear.HalClient.Unit.Tests
{
    internal class HalClientTestContext
    {
        public const string RootUri = "/v1/version/1";
        public const string Curie = "retail";
        public Guid OrderRef => _order.OrderRef;
        public OrderAdd OrderAdd { get; }
        public OrderEdit OrderEdit { get; }
        public static readonly Guid UserRef = Guid.NewGuid();

        private readonly IHalClient _sut;
        private readonly IJsonHttpClient _http;
        private IHalClient _result;
        private readonly dynamic _curies;
        private readonly Version _version;
        private readonly Order _order;
        private readonly OrderItem _orderItem;
        private readonly PagedList _paged;

        public HalClientTestContext()
        {
            var fixture = new Fixture().Customize(new AutoRhinoMockCustomization());

            _http = fixture.Freeze<IJsonHttpClient>();

            _version = fixture.Create<Version>();
            _order = fixture.Create<Order>();
            _orderItem = fixture.Create<OrderItem>();
            _paged = fixture.Create<PagedList>();
            OrderAdd = fixture.Create<OrderAdd>();
            OrderEdit = fixture.Create<OrderEdit>();

            _sut = new HalClient(_http);

            _curies =
                new[]
                {
                    new
                    {
                        href = "https://retail.com/v1/docs/{rel}",
                        name = Curie,
                        templated = true
                    }
                };
        }

        public HalClientTestContext ArrangeHomeResource()
        {
            ArrangeGet(RootUri, CreateRootResourceJson());

            return this;
        }

        public HalClientTestContext ArrangeSingleResource()
        {
            ArrangeGet($"/v1/order/{OrderRef}", CreateSingleResourceJson());

            return this;
        }

        public void ArrangePagedResource()
        {
            ArrangeGet($"/v1/order?userRef={UserRef}", CreatePagedResourceJson());
        }

        public void ArrangeCreatedResource()
        {
            ArrangePost("/v1/order", CreateSingleResourceJson());
        }

        public void ArrangeUpdatedResource()
        {
            ArrangePut($"/v1/order/{OrderRef}", CreateSingleResourceJson());
        }

        public void ArrangeDeletedResource()
        {
            ArrangeDelete($"/v1/order/{OrderRef}");
        }

        private void ArrangeGet(string uri, object content)
        {
            _http
                .Expect(h => h.GetAsync(uri))
                .Return(Ok(content));
        }

        private void ArrangePost(string uri, object content)
        {
            _http
                .Expect(h => h.PostAsync(Arg<string>.Is.Equal(uri), Arg<object>.Is.Anything))
                .Return(Ok(content))
                .IgnoreArguments();
        }

        private void ArrangePut(string uri, object content)
        {
            _http
                .Expect(h => h.PutAsync(Arg<string>.Is.Equal(uri), Arg<object>.Is.Anything))
                .Return(Ok(content));
        }

        private void ArrangeDelete(string uri)
        {
            _http
                .Expect(h => h.DeleteAsync(uri))
                .Return(Ok());
        }

        public void Act(Func<IHalClient, IHalClient> act)
        {
            _result = act(_sut);
        }

        public void AssertThatRootResourceIsPresent()
        {
            var resource = _result.Item<Version>().Data;
            _version.AsSource().OfLikeness<Version>().ShouldEqual(resource);
        }

        public void AssertThatSingleResourceIsPresent()
        {
            var resource = _result.Item<Order>().Data;
            _order.AsSource().OfLikeness<Order>().ShouldEqual(resource);
        }

        public void AssertThatPagedResourceIsPresent()
        {
            var resource = _result.Item<PagedList>().Data;
            _paged.AsSource().OfLikeness<PagedList>().ShouldEqual(resource);
        }

        public void AssertThatEmbeddedPagedResourceIsPresent()
        {
            var resource = _result.Items<Order>().Data().First();
            _order.AsSource().OfLikeness<Order>().ShouldEqual(resource);
        }

        public void AssertThatSingleEmbeddedResourceIsPresent()
        {
            var resource = _result.Items<OrderItem>().Data().First();
            _orderItem.AsSource().OfLikeness<OrderItem>().ShouldEqual(resource);
        }

        public void AssertThatResourceWasCreated()
        {
            var resource = _result.Item<Order>().Data;
            _order.AsSource().OfLikeness<Order>().ShouldEqual(resource);
        }

        public void AssertThatResourceWasUpdated()
        {
            var resource = _result.Item<Order>().Data;
            _order.AsSource().OfLikeness<Order>().ShouldEqual(resource);
        }

        public void AssertThatResourceWasDeleted()
        {
            _http.VerifyAllExpectations();
        }

        public void AssertThatResourceHasRelationship()
        {
            _sut.Has("order-edit", Curie).Should().BeTrue("Resource should have relationship");
        }

        public void AssertThatResourceDoesNotHasRelationship()
        {
            _sut.Has("whatever", Curie).Should().BeFalse("Resource should not have relationship");
        }

        private static Task<HttpResponseMessage> Ok() =>
            Task<HttpResponseMessage>
                .Factory
                .StartNew(() => new HttpResponseMessage(HttpStatusCode.OK));

        private static Task<HttpResponseMessage> Ok(object content) =>
            Task<HttpResponseMessage>
                .Factory
                .StartNew(
                    () =>
                        new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = Serialize(content)
                        });

        private static HttpContent Serialize(object data)
        {
            var serializer = new JsonSerializer();
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, data);
                var content =
                    writer
                        .ToString()
                        .Replace($"{Curie}_", $"{Curie}:")
                        .Replace("_", "-")
                        .Replace("-links", "_links")
                        .Replace("-embedded", "_embedded");
                return new StringContent(content, Encoding.UTF8, "application/json");
            }
        }

        private object CreateRootResourceJson() =>
            new
            {
                _version.VersionNumber,
                _links =
                    new
                    {
                        curies = _curies,
                        self = new {href = RootUri},
                        retail_order = new {href = "/v1/order/{orderRef}", templated = true},
                        retail_order_queryby_user = new {href = "/v1/order?userRef={userRef}", templated = true},
                        retail_order_add = new {href = "/v1/order"}
                    }
            };

        private object CreateSingleResourceJson() =>
            new
            {
                _order.OrderRef,
                _order.OrderNumber,
                _order.Status,
                _order.Total,
                _links =
                    new
                    {
                        curies = _curies,
                        self = new {href = $"/v1/order/{OrderRef}"},
                        retail_order_edit = new {href = $"/v1/order/{OrderRef}"},
                        retail_order_delete = new {href = $"/v1/order/{OrderRef}"}
                    },
                _embedded =
                    new
                    {
                        retail_orderitem =
                            new[]
                            {
                                new
                                {
                                    _orderItem.OrderItemRef,
                                    _orderItem.Status,
                                    _orderItem.Total,
                                    _orderItem.Quantity,
                                    _links =
                                        new
                                        {
                                            curies = _curies,
                                            self = new {href = $"/v1/orderitem/{_orderItem.OrderItemRef}"}
                                        }
                                }
                            }
                    }
            };

        private object CreatePagedResourceJson() =>
            new
            {
                _paged.PageNumber,
                _paged.PageSize,
                _paged.KnownPagesAvailable,
                _paged.TotalItemsCount,
                _links =
                    new
                    {
                        curies = _curies,
                        self = new {href = $"/v1/order?userRef={UserRef}"}
                    },
                _embedded =
                    new
                    {
                        retail_order =
                            new[]
                            {
                                new
                                {
                                    _order.OrderRef,
                                    _order.OrderNumber,
                                    _order.Status,
                                    _order.Total,
                                    _links =
                                        new
                                        {
                                            curies = _curies,
                                            self = new {href = $"/v1/order/{OrderRef}"}
                                        }
                                }
                            }
                    }
            };
    }
}