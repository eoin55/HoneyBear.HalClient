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
using NUnit.Framework;
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
        private bool _hasCurie;
        private readonly IFixture _fixture;

        public HalClientTestContext()
        {
            _fixture = new Fixture().Customize(new AutoRhinoMockCustomization());

            _http = _fixture.Freeze<IJsonHttpClient>();

            _version = _fixture.Create<Version>();
            _order = _fixture.Create<Order>();
            _orderItem = _fixture.Create<OrderItem>();
            _paged = _fixture.Create<PagedList>();
            OrderAdd = _fixture.Create<OrderAdd>();
            OrderEdit = _fixture.Create<OrderEdit>();

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
            _hasCurie = true;
        }

        public HalClientTestContext ArrangeWithoutCurie()
        {
            _hasCurie = false;

            return this;
        }

        public HalClientTestContext ArrangeHomeResource()
        {
            var content = _hasCurie ? CreateRootResourceJson() : CreateRootResourceJsonWithoutCurie();
            ArrangeGet(RootUri, content);

            return this;
        }

        public HalClientTestContext ArrangeDefaultHomeResource()
        {
            var content = _hasCurie ? CreateRootResourceJson() : CreateRootResourceJsonWithoutCurie();
            ArrangeGet(string.Empty, content);

            return this;
        }

        public HalClientTestContext ArrangeSingleResource()
        {
            var content = _hasCurie ? CreateSingleResourceJson() : CreateSingleResourceJsonWithoutCurie();
            ArrangeGet($"/v1/order/{OrderRef}", content);

            return this;
        }

        public void ArrangePagedResource()
        {
            ArrangeGet($"/v1/order?userRef={UserRef}", CreatePagedResourceJson());
        }

        public void ArrangeDefaultPagedResource()
        {
            ArrangeGet($"/v1/order", CreateDefaultPagedResourceJson());
        }

        public void ArrangeCreatedResource()
        {
            var content = _hasCurie ? CreateSingleResourceJson() : CreateSingleResourceJsonWithoutCurie();
            ArrangePost("/v1/order", content);
        }

        public void ArrangeUpdatedResource()
        {
            var content = _hasCurie ? CreateSingleResourceJson() : CreateSingleResourceJsonWithoutCurie();
            ArrangePut($"/v1/order/{OrderRef}", content);
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

        public void ArrangeFailedHomeRequest()
        {
            _http
                .Expect(h => h.GetAsync(RootUri))
                .Return(NotFound());
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

        public void AssertThatResourceHasRelationshipWithoutCurie()
        {
            _sut.Has("order-edit").Should().BeTrue("Resource should have relationship");
        }

        public void AssertThatResourceDoesNotHasRelationship()
        {
            _sut.Has("whatever", Curie).Should().BeFalse("Resource should not have relationship");
        }

        public void AssertThatResourceDoesNotHasRelationshipWithoutCurie()
        {
            _sut.Has("whatever").Should().BeFalse("Resource should not have relationship");
        }

        public void AssertThatHttpClientCanBeProvided()
        {
            var baseAddress = _fixture.Create<Uri>();
            var http = new HttpClient {BaseAddress = baseAddress};
            var sut = new HalClient(http);
            sut.HttpClient.BaseAddress.Should().Be(baseAddress);
        }

        public void AssertThatDefaultHttpClientCanBeUsed()
        {
            var sut = new HalClient();
            sut.HttpClient.BaseAddress.Should().BeNull("Because it hasn't been set.");
        }

        public void AssertThatResolvingResourceThrowsExceptionWhenResourceNotNavigated()
        {
            Assert.Throws<NoActiveResource>(() => _sut.Item<Order>());
        }

        private static Task<HttpResponseMessage> Ok() =>
            Task<HttpResponseMessage>
                .Factory
                .StartNew(() => new HttpResponseMessage(HttpStatusCode.OK));

        private static Task<HttpResponseMessage> NotFound() =>
            Task<HttpResponseMessage>
                .Factory
                .StartNew(() => new HttpResponseMessage(HttpStatusCode.NotFound));

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
                        retail_order_add = new {href = "/v1/order"},
                        retail_order_add_with_id = new {href = "/v1/order/{orderRef}", templated = true},
                        retail_order_edit = new {href = "/v1/order/{orderRef}", templated = true},
                        retail_order_delete = new {href = "/v1/order/{orderRef}", templated = true}
                    }
            };

        private object CreateRootResourceJsonWithoutCurie() =>
            new
            {
                _version.VersionNumber,
                _links =
                    new
                    {
                        self = new {href = RootUri},
                        order = new {href = "/v1/order/{orderRef}", templated = true},
                        order_queryby_user = new {href = "/v1/order?userRef={userRef}", templated = true},
                        order_query_all = new {href = "/v1/order"},
                        order_add = new {href = "/v1/order"}
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

        private object CreateSingleResourceJsonWithoutCurie() =>
            new
            {
                _order.OrderRef,
                _order.OrderNumber,
                _order.Status,
                _order.Total,
                _links =
                    new
                    {
                        self = new {href = $"/v1/order/{OrderRef}"},
                        order_edit = new {href = $"/v1/order/{OrderRef}"},
                        order_delete = new {href = $"/v1/order/{OrderRef}"}
                    },
                _embedded =
                    new
                    {
                        orderitem =
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

        private object CreateDefaultPagedResourceJson() =>
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
                        self = new {href = $"/v1/order"}
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