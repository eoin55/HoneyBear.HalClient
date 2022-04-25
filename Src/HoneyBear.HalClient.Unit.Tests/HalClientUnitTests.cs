using HoneyBear.HalClient.Models;
using HoneyBear.HalClient.Unit.Tests.ProxyResources;
using NUnit.Framework;

namespace HoneyBear.HalClient.Unit.Tests
{
    [TestFixture]
    internal sealed class HalClientUnitTests
    {
        private const string Curie = HalClientTestContext.Curie;
        private const string RootUri = HalClientTestContext.RootUri;
        private HalClientTestContext _context;

        [SetUp]
        public void SetUp() => _context = new HalClientTestContext();

        [Test]
        public void Navigate_to_root_resource()
        {
            _context.ArrangeHomeResource();

            _context.Act(sut => sut.Root(RootUri));

            _context.AssertThatRootResourceIsPresent();
        }

        [Test]
        public void Navigate_to_single_resource()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Get("order", new {orderRef = _context.OrderRef}, Curie);

            _context.Act(Act);

            _context.AssertThatSingleResourceIsPresent();
        }

        [Test]
        public void Navigate_to_single_embedded_resource()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Get("order", new {orderRef = _context.OrderRef}, Curie)
                .Get("orderitem-query", Curie);

            _context.Act(Act);

            _context.AssertThatSingleEmbeddedResourceIsPresent();
        }

        [Test]
        public void Navigate_to_paged_resource()
        {
            _context
                .ArrangeHomeResource()
                .ArrangePagedResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Get("order-queryby-user", new {userRef = HalClientTestContext.UserRef}, Curie);

            _context.Act(Act);

            _context.AssertThatPagedResourceIsPresent();
        }

        [Test]
        public void Navigate_to_paged_embedded_resource()
        {
            _context
                .ArrangeHomeResource()
                .ArrangePagedResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Get("order-queryby-user", new {userRef = HalClientTestContext.UserRef}, Curie)
                .Get("order", new {orderRef = _context.OrderRef}, Curie);

            _context.Act(Act);

            _context.AssertThatEmbeddedPagedResourceIsPresent();
        }

        [Test]
        public void Navigate_to_paged_embedded_resource_and_navigate_to_embedded_resource_via_parent()
        {
            _context
                .ArrangeHomeResource()
                .ArrangePagedResource();

            IHalClient Act(IHalClient sut)
            {
                var resource =
                    sut
                        .Root(RootUri)
                        .Get("order-queryby-user", new {userRef = HalClientTestContext.UserRef}, Curie)
                        .Item<PagedList>();

                return sut.Get(resource, "order", new {orderRef = _context.OrderRef}, Curie);
            }

            _context.Act(Act);

            _context.AssertThatSingleResourceIsPresent();
        }

        [Test]
        public void Navigate_to_paged_linked_resource_and_navigate_to_linked_resource_via_parent()
        {
            _context
                .ArrangeHomeResource()
                .ArrangePagedResourceWithLinkedResources()
                .ArrangeSingleResource();

            IHalClient Act(IHalClient sut)
            {
                var resource =
                    sut
                        .Root(RootUri)
                        .Get("order-queryby-user", new {userRef = HalClientTestContext.UserRef}, Curie)
                        .Item<PagedList>();

                return sut.Get(resource, "order", new {orderRef = _context.OrderRef}, Curie);
            }

            _context.Act(Act);

            _context.AssertThatSingleResourceIsPresent();
        }

        [Test]
        public void Navigate_to_paged_embedded_resource_and_navigate_to_embedded_resource_array()
        {
            _context
                .ArrangeHomeResource()
                .ArrangePagedResourceWithEmbeddedArrayOfResources();

            IHalClient Act(IHalClient sut)
            {
                var order =
                    sut
                        .Root(RootUri)
                        .Get("order-queryby-user", new {userRef = HalClientTestContext.UserRef}, Curie)
                        .Get("order", new {orderRef = _context.OrderRef}, Curie)
                        .Item<Order>();

                return
                    sut
                        .Get(order, "orderitem-query", Curie)
                        .Get("orderitem", new {orderItemRef = _context.OrderItemRef}, Curie);
            }

            _context.Act(Act);

            _context.AssertThatResourceArrayIsPresent();
        }

        [Test]
        public void Navigate_to_paged_embedded_resource_and_navigate_to_linked_resource_array()
        {
            _context
                .ArrangeHomeResource()
                .ArrangePagedResourceWithLinkedArrayOfResources();

            IHalClient Act(IHalClient sut)
            {
                var order =
                    sut
                        .Root(RootUri)
                        .Get("order-queryby-user", new {userRef = HalClientTestContext.UserRef}, Curie)
                        .Get("order", new {orderRef = _context.OrderRef}, Curie)
                        .Item<Order>();

                return
                    sut
                        .Get(order, "orderitem-query", Curie)
                        .Get("orderitem", new {orderItemRef = _context.OrderItemRef}, Curie);
            }

            _context.Act(Act);

            _context.AssertThatResourceArrayIsPresent();
        }

        [Test]
        public void Navigate_to_resource_with_JSON_attribute()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeResourceWithJsonAttribute();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Get("resource-with-json-attribute", null, Curie);

            _context.Act(Act);

            _context.AssertThatResourceWithJsonAttributeIsPresent();
        }

        [Test]
        public void Create_resource()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource()
                .ArrangeCreatedResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Post("order-add", _context.OrderAdd, Curie);

            _context.Act(Act);

            _context.AssertThatResourceWasCreated();
        }

        [Test]
        public void Create_resource_with_parameters()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource()
                .ArrangeCreatedResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Post("order-add", _context.OrderAdd, new {orderRef = _context.OrderRef}, Curie);

            _context.Act(Act);

            _context.AssertThatResourceWasCreated();
        }

        [Test]
        public void Create_resource_without_curie()
        {
            _context
                .ArrangeWithoutCurie()
                .ArrangeHomeResource()
                .ArrangeSingleResource()
                .ArrangeCreatedResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Post("order-add", _context.OrderAdd);

            _context.Act(Act);

            _context.AssertThatResourceWasCreated();
        }

        [Test]
        public void Create_resource_with_parameters_without_curie()
        {
            _context
                .ArrangeWithoutCurie()
                .ArrangeHomeResource()
                .ArrangeCreatedResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Post("order-add", _context.OrderAdd, new {orderRef = _context.OrderRef});

            _context.Act(Act);

            _context.AssertThatResourceWasCreated();
        }

        [Test]
        public void Update_resource()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource()
                .ArrangeUpdatedResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Get("order", new {orderRef = _context.OrderRef}, Curie)
                .Put("order-edit", _context.OrderEdit, Curie);

            _context.Act(Act);

            _context.AssertThatResourceWasUpdated();
        }

        [Test]
        public void Update_resource_with_parameters()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource()
                .ArrangeUpdatedResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Put("order-edit", _context.OrderEdit, new {orderRef = _context.OrderRef}, Curie);

            _context.Act(Act);

            _context.AssertThatResourceWasUpdated();
        }

        [Test]
        public void Update_resource_without_curie()
        {
            _context
                .ArrangeWithoutCurie()
                .ArrangeHomeResource()
                .ArrangeSingleResource()
                .ArrangeUpdatedResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Get("order", new {orderRef = _context.OrderRef})
                .Put("order-edit", _context.OrderEdit);

            _context.Act(Act);

            _context.AssertThatResourceWasUpdated();
        }

        [Test]
        public void Update_resource_with_parameters_without_curie()
        {
            _context
                .ArrangeWithoutCurie()
                .ArrangeHomeResource()
                .ArrangeUpdatedResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Put("order-edit", _context.OrderEdit, new {orderRef = _context.OrderRef});

            _context.Act(Act);

            _context.AssertThatResourceWasUpdated();
        }

        [Test]
        public void Patch_resource()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource()
                .ArrangePatchedResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Get("order", new {orderRef = _context.OrderRef}, Curie)
                .Patch("order-edit", _context.OrderEdit, Curie);

            _context.Act(Act);

            //For simplicty we don't actually apply any patch here, 
            //but just test if the payload arrived via PATCH
            _context.AssertThatResourceWasUpdated();
        }

        [Test]
        public void Patch_resource_with_parameters()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource()
                .ArrangePatchedResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Patch("order-edit", _context.OrderEdit, new {orderRef = _context.OrderRef}, Curie);

            _context.Act(Act);
            
            //For simplicty we don't actually apply any patch here, 
            //but just test if the payload arrived via PATCH
            _context.AssertThatResourceWasUpdated();
        }

        [Test]
        public void Patch_resource_without_curie()
        {
            _context
                .ArrangeWithoutCurie()
                .ArrangeHomeResource()
                .ArrangeSingleResource()
                .ArrangePatchedResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Get("order", new {orderRef = _context.OrderRef})
                .Patch("order-edit", _context.OrderEdit);

            _context.Act(Act);

            //For simplicty we don't actually apply any patch here, 
            //but just test if the payload arrived via PATCH
            _context.AssertThatResourceWasUpdated();
        }

        [Test]
        public void Patch_resource_with_parameters_without_curie()
        {
            _context
                .ArrangeWithoutCurie()
                .ArrangeHomeResource()
                .ArrangePatchedResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Patch("order-edit", _context.OrderEdit, new {orderRef = _context.OrderRef});

            _context.Act(Act);

            //For simplicty we don't actually apply any patch here, 
            //but just test if the payload arrived via PATCH
            _context.AssertThatResourceWasUpdated();
        }

        [Test]
        public void Delete_resource()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource()
                .ArrangeDeletedResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Get("order", new {orderRef = _context.OrderRef}, Curie)
                .Delete("order-delete", Curie);

            _context.Act(Act);

            _context.AssertThatResourceWasDeleted();
        }

        [Test]
        public void Delete_resource_with_parameters()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeDeletedResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Delete("order-delete", new {orderRef = _context.OrderRef}, Curie);

            _context.Act(Act);

            _context.AssertThatResourceWasDeleted();
        }

        [Test]
        public void Delete_resource_without_curie()
        {
            _context
                .ArrangeWithoutCurie()
                .ArrangeHomeResource()
                .ArrangeSingleResource()
                .ArrangeDeletedResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Get("order", new {orderRef = _context.OrderRef})
                .Delete("order-delete");

            _context.Act(Act);

            _context.AssertThatResourceWasDeleted();
        }

        [Test]
        public void Delete_resource_with_parameters_without_curie()
        {
            _context
                .ArrangeWithoutCurie()
                .ArrangeHomeResource()
                .ArrangeDeletedResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Delete("order-delete", new {orderRef = _context.OrderRef});

            _context.Act(Act);

            _context.AssertThatResourceWasDeleted();
        }

        [Test]
        public void Has_relationship()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Get("order", new {orderRef = _context.OrderRef}, Curie);

            _context.Act(Act);

            _context.AssertThatResourceHasRelationship();
        }

        [Test]
        public void Has_relationship_without_curie()
        {
            _context
                .ArrangeWithoutCurie()
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Get("order", new {orderRef = _context.OrderRef});

            _context.Act(Act);

            _context.AssertThatResourceHasRelationshipWithoutCurie();
        }

        [Test]
        public void Does_not_have_relationship()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Get("order", new {orderRef = _context.OrderRef}, Curie);

            _context.Act(Act);

            _context.AssertThatResourceDoesNotHasRelationship();
        }

        [Test]
        public void Does_not_have_relationship_without_curie()
        {
            _context
                .ArrangeWithoutCurie()
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Get("order", new {orderRef = _context.OrderRef});

            _context.Act(Act);

            _context.AssertThatResourceDoesNotHasRelationshipWithoutCurie();
        }

        [Test]
        public void Navigate_to_single_resource_without_curie()
        {
            _context
                .ArrangeWithoutCurie()
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Get("order", new {orderRef = _context.OrderRef});

            _context.Act(Act);

            _context.AssertThatSingleResourceIsPresent();
        }

        [Test]
        public void Navigate_to_default_paged_resource_without_curie()
        {
            _context
                .ArrangeWithoutCurie()
                .ArrangeHomeResource()
                .ArrangeDefaultPagedResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Get("order-query-all");

            _context.Act(Act);

            _context.AssertThatPagedResourceIsPresent();
        }

        [Test]
        public void HalClient_can_be_created_with_specified_HttpClient() =>
            _context.AssertThatHttpClientCanBeProvided();

        [Test]
        public void HalClient_can_be_created_with_default_HttpClient() =>
            _context.AssertThatDefaultHttpClientCanBeUsed();

        [Test]
        public void Navigate_to_default_home_resource()
        {
            _context
                .ArrangeDefaultHomeResource()
                .ArrangeSingleResource();

            IHalClient Act(IHalClient sut) => sut
                .Root()
                .Get("order", new {orderRef = _context.OrderRef}, Curie);

            _context.Act(Act);

            _context.AssertThatSingleResourceIsPresent();
        }

        [Test]
        public void Throws_exception_when_template_parameters_are_not_passed()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Get("order", Curie);

            Assert.Throws<TemplateParametersAreRequired>(() => _context.Act(Act));
        }

        [Test]
        public void Throws_exception_when_relationship_does_not_exist()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Get("I-do-not-exist", Curie);

            Assert.Throws<FailedToResolveRelationship>(() => _context.Act(Act));
        }

        [Test]
        public void Resolving_resource_throws_an_exception_when_the_resource_has_not_been_navigated() =>
            _context.AssertThatResolvingResourceThrowsExceptionWhenResourceNotNavigated();

        [Test]
        public void Throws_exception_when_HTTP_request_is_unsuccessful()
        {
            _context.ArrangeFailedHomeRequest();

            IHalClient Act(IHalClient sut) => sut.Root(RootUri);

            Assert.Throws<HttpRequestFailed>(() => _context.Act(Act));
        }

        [Test]
        public void Throws_exception_when_resource_does_not_exist()
        {
            _context
                .ArrangeHomeResource()
                .ArrangePagedResource()
                .ArrangeFailedOrderRequest();

            IHalClient Act(IHalClient sut) => sut
                .Root(RootUri)
                .Get("order-queryby-user", new {userRef = HalClientTestContext.UserRef}, Curie)
                .Get("order", new {orderRef = HalClientTestContext.NonExistentOrderRef}, Curie);

            Assert.Throws<HttpRequestFailed>(() => _context.Act(Act));
        }
    }
}