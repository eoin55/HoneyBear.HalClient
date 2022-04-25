using System;
using System.Threading.Tasks;
using HoneyBear.HalClient.Unit.Tests.ProxyResources;
using NUnit.Framework;

namespace HoneyBear.HalClient.Unit.Tests
{
    [TestFixture]
    internal sealed class HalClientAsyncUnitTests
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

            _context.ActAsync(sut => sut.RootAsync(RootUri));

            _context.AssertThatRootResourceIsPresent();
        }

        [Test]
        public void Navigate_to_single_resource()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .GetAsync("order", new {orderRef = _context.OrderRef}, Curie);

            _context.ActAsync(Act);

            _context.AssertThatSingleResourceIsPresent();
        }

        [Test]
        public void Navigate_to_single_embedded_resource()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .GetAsync("order", new {orderRef = _context.OrderRef}, Curie)
                .GetAsync("orderitem-query", Curie);

            _context.ActAsync(Act);

            _context.AssertThatSingleEmbeddedResourceIsPresent();
        }

        [Test]
        public void Navigate_to_paged_resource()
        {
            _context
                .ArrangeHomeResource()
                .ArrangePagedResource();

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .GetAsync("order-queryby-user", new {userRef = HalClientTestContext.UserRef}, Curie);

            _context.ActAsync(Act);

            _context.AssertThatPagedResourceIsPresent();
        }

        [Test]
        public void Navigate_to_paged_embedded_resource()
        {
            _context
                .ArrangeHomeResource()
                .ArrangePagedResource();

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .GetAsync("order-queryby-user", new {userRef = HalClientTestContext.UserRef}, Curie)
                .GetAsync("order", new {orderRef = _context.OrderRef}, Curie);

            _context.ActAsync(Act);

            _context.AssertThatEmbeddedPagedResourceIsPresent();
        }

        [Test]
        public void Navigate_to_paged_embedded_resource_and_navigate_to_embedded_resource_via_parent()
        {
            _context
                .ArrangeHomeResource()
                .ArrangePagedResource();

            Task<IHalClient> Act(IHalClient sut)
            {
                var resource =
                    sut
                        .RootAsync(RootUri)
                        .GetAsync("order-queryby-user", new {userRef = HalClientTestContext.UserRef}, Curie)
                        .Item<PagedList>();

                return sut.GetAsync(resource, "order", new {orderRef = _context.OrderRef}, Curie);
            }

            _context.ActAsync(Act);

            _context.AssertThatSingleResourceIsPresent();
        }

        [Test]
        public void Navigate_to_paged_linked_resource_and_navigate_to_linked_resource_via_parent()
        {
            _context
                .ArrangeHomeResource()
                .ArrangePagedResourceWithLinkedResources()
                .ArrangeSingleResource();

            Task<IHalClient> Act(IHalClient sut)
            {
                var resource =
                    sut
                        .RootAsync(RootUri)
                        .GetAsync("order-queryby-user", new {userRef = HalClientTestContext.UserRef}, Curie)
                        .Item<PagedList>();

                return sut.GetAsync(resource, "order", new {orderRef = _context.OrderRef}, Curie);
            }

            _context.ActAsync(Act);

            _context.AssertThatSingleResourceIsPresent();
        }

        [Test]
        public void Navigate_to_paged_embedded_resource_and_navigate_to_embedded_resource_array()
        {
            _context
                .ArrangeHomeResource()
                .ArrangePagedResourceWithEmbeddedArrayOfResources();

            Task<IHalClient> Act(IHalClient sut)
            {
                var nav =
                    sut
                        .RootAsync(RootUri)
                        .GetAsync("order-queryby-user", new {userRef = HalClientTestContext.UserRef}, Curie)
                        .GetAsync("order", Curie);

                var order = nav.Item<Order>();

                return
                    nav
                        .GetAsync(order, "orderitem-query", Curie)
                        .GetAsync("orderitem", new {orderItemRef = _context.OrderItemRef}, Curie);
            }

            _context.ActAsync(Act);

            _context.AssertThatResourceArrayIsPresent();
        }

        [Test]
        public void Navigate_to_paged_embedded_resource_and_navigate_to_linked_resource_array()
        {
            _context
                .ArrangeHomeResource()
                .ArrangePagedResourceWithLinkedArrayOfResources();

            Task<IHalClient> Act(IHalClient sut)
            {
                var nav =
                    sut
                        .RootAsync(RootUri)
                        .GetAsync("order-queryby-user", new {userRef = HalClientTestContext.UserRef}, Curie)
                        .GetAsync("order", new {orderRef = _context.OrderRef}, Curie);

                var order = nav.Item<Order>();

                return
                    nav
                        .GetAsync(order, "orderitem-query", Curie)
                        .GetAsync("orderitem", new {orderItemRef = _context.OrderItemRef}, Curie);
            }

            _context.ActAsync(Act);

            _context.AssertThatResourceArrayIsPresent();
        }

        [Test]
        public void Navigate_to_resource_with_JSON_attribute()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeResourceWithJsonAttribute();

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .GetAsync("resource-with-json-attribute", null, Curie);

            _context.ActAsync(Act);

            _context.AssertThatResourceWithJsonAttributeIsPresent();
        }

        [Test]
        public void Create_resource()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource()
                .ArrangeCreatedResource();

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .PostAsync("order-add", _context.OrderAdd, Curie);

            _context.ActAsync(Act);

            _context.AssertThatResourceWasCreated();
        }

        [Test]
        public void Create_resource_with_parameters()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource()
                .ArrangeCreatedResource();

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .PostAsync("order-add", _context.OrderAdd, new {orderRef = _context.OrderRef}, Curie);

            _context.ActAsync(Act);

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

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .PostAsync("order-add", _context.OrderAdd);

            _context.ActAsync(Act);

            _context.AssertThatResourceWasCreated();
        }

        [Test]
        public void Create_resource_with_parameters_without_curie()
        {
            _context
                .ArrangeWithoutCurie()
                .ArrangeHomeResource()
                .ArrangeCreatedResource();

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .PostAsync("order-add", _context.OrderAdd, new {orderRef = _context.OrderRef});

            _context.ActAsync(Act);

            _context.AssertThatResourceWasCreated();
        }

        [Test]
        public void Update_resource()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource()
                .ArrangeUpdatedResource();

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .GetAsync("order", new {orderRef = _context.OrderRef}, Curie)
                .PutAsync("order-edit", _context.OrderEdit, Curie);

            _context.ActAsync(Act);

            _context.AssertThatResourceWasUpdated();
        }

        [Test]
        public void Update_resource_with_parameters()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource()
                .ArrangeUpdatedResource();

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .PutAsync("order-edit", _context.OrderEdit, new {orderRef = _context.OrderRef}, Curie);

            _context.ActAsync(Act);

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

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .GetAsync("order", new {orderRef = _context.OrderRef})
                .PutAsync("order-edit", _context.OrderEdit);

            _context.ActAsync(Act);

            _context.AssertThatResourceWasUpdated();
        }

        [Test]
        public void Update_resource_with_parameters_without_curie()
        {
            _context
                .ArrangeWithoutCurie()
                .ArrangeHomeResource()
                .ArrangeUpdatedResource();

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .PutAsync("order-edit", _context.OrderEdit, new {orderRef = _context.OrderRef});

            _context.ActAsync(Act);

            _context.AssertThatResourceWasUpdated();
        }

        [Test]
        public void Patch_resource()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource()
                .ArrangePatchedResource();

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .GetAsync("order", new {orderRef = _context.OrderRef}, Curie)
                .PatchAsync("order-edit", _context.OrderEdit, Curie);

            _context.ActAsync(Act);

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

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .PatchAsync("order-edit", _context.OrderEdit, new {orderRef = _context.OrderRef}, Curie);

            _context.ActAsync(Act);
            
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

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .GetAsync("order", new {orderRef = _context.OrderRef})
                .PatchAsync("order-edit", _context.OrderEdit);

            _context.ActAsync(Act);

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

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .PatchAsync("order-edit", _context.OrderEdit, new {orderRef = _context.OrderRef});

            _context.ActAsync(Act);

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

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .GetAsync("order", new {orderRef = _context.OrderRef}, Curie)
                .DeleteAsync("order-delete", Curie);

            _context.ActAsync(Act);

            _context.AssertThatResourceWasDeleted();
        }

        [Test]
        public void Delete_resource_with_parameters()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeDeletedResource();

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .DeleteAsync("order-delete", new {orderRef = _context.OrderRef}, Curie);

            _context.ActAsync(Act);

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

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .GetAsync("order", new {orderRef = _context.OrderRef})
                .DeleteAsync("order-delete");

            _context.ActAsync(Act);

            _context.AssertThatResourceWasDeleted();
        }

        [Test]
        public void Delete_resource_with_parameters_without_curie()
        {
            _context
                .ArrangeWithoutCurie()
                .ArrangeHomeResource()
                .ArrangeDeletedResource();

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .DeleteAsync("order-delete", new {orderRef = _context.OrderRef});

            _context.ActAsync(Act);

            _context.AssertThatResourceWasDeleted();
        }

        [Test]
        public void Has_relationship()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .GetAsync("order", new {orderRef = _context.OrderRef}, Curie);

            _context.ActAsync(Act);

            _context.AssertThatResourceHasRelationship();
        }

        [Test]
        public void Has_relationship_without_curie()
        {
            _context
                .ArrangeWithoutCurie()
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .GetAsync("order", new {orderRef = _context.OrderRef});

            _context.ActAsync(Act);

            _context.AssertThatResourceHasRelationshipWithoutCurie();
        }

        [Test]
        public void Does_not_have_relationship()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .GetAsync("order", new {orderRef = _context.OrderRef}, Curie);

            _context.ActAsync(Act);

            _context.AssertThatResourceDoesNotHasRelationship();
        }

        [Test]
        public void Does_not_have_relationship_without_curie()
        {
            _context
                .ArrangeWithoutCurie()
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .GetAsync("order", new {orderRef = _context.OrderRef});

            _context.ActAsync(Act);

            _context.AssertThatResourceDoesNotHasRelationshipWithoutCurie();
        }

        [Test]
        public void Navigate_to_single_resource_without_curie()
        {
            _context
                .ArrangeWithoutCurie()
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .GetAsync("order", new {orderRef = _context.OrderRef});

            _context.ActAsync(Act);

            _context.AssertThatSingleResourceIsPresent();
        }

        [Test]
        public void Navigate_to_default_paged_resource_without_curie()
        {
            _context
                .ArrangeWithoutCurie()
                .ArrangeHomeResource()
                .ArrangeDefaultPagedResource();

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .GetAsync("order-query-all");

            _context.ActAsync(Act);

            _context.AssertThatPagedResourceIsPresent();
        }

        [Test]
        public void HalClientAsync_can_be_created_with_specified_HttpClient() =>
            _context.AssertThatHttpClientCanBeProvided();

        [Test]
        public void HalClientAsync_can_be_created_with_default_HttpClient() =>
            _context.AssertThatDefaultHttpClientCanBeUsed();

        [Test]
        public void Navigate_to_default_home_resource()
        {
            _context
                .ArrangeDefaultHomeResource()
                .ArrangeSingleResource();

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync()
                .GetAsync("order", new {orderRef = _context.OrderRef}, Curie);

            _context.ActAsync(Act);

            _context.AssertThatSingleResourceIsPresent();
        }

        [Test]
        public void Throws_exception_when_template_parameters_are_not_passed()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .GetAsync("order", Curie);

            Assert.Throws<AggregateException>(() => _context.ActAsync(Act));
        }

        [Test]
        public void Throws_exception_when_relationship_does_not_exist()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .GetAsync("I-do-not-exist", Curie);

            Assert.Throws<AggregateException>(() => _context.ActAsync(Act));
        }

        [Test]
        public void Resolving_resource_throws_an_exception_when_the_resource_has_not_been_navigated() =>
            _context.AssertThatResolvingResourceThrowsExceptionWhenResourceNotNavigated();

        [Test]
        public void Throws_exception_when_HTTP_request_is_unsuccessful()
        {
            _context.ArrangeFailedHomeRequest();

            Task<IHalClient> Act(IHalClient sut) => sut.RootAsync(RootUri);

            Assert.Throws<AggregateException>(() => _context.ActAsync(Act));
        }

        [Test]
        public void Throws_exception_when_resource_does_not_exist()
        {
            _context
                .ArrangeHomeResource()
                .ArrangePagedResource()
                .ArrangeFailedOrderRequest();

            Task<IHalClient> Act(IHalClient sut) => sut
                .RootAsync(RootUri)
                .GetAsync("order-queryby-user", new { userRef = HalClientTestContext.UserRef }, Curie)
                .GetAsync("order", new { orderRef = HalClientTestContext.NonExistentOrderRef }, Curie);

            Assert.Throws<AggregateException>(() => _context.ActAsync(Act));
        }
    }
}