using System;
using System.Net.Http;
using HoneyBear.HalClient.Models;
using NUnit.Framework;

namespace HoneyBear.HalClient.Unit.Tests
{
    [TestFixture]
    internal class HalClientUnitTests
    {
        private HalClientTestContext _context;

        [SetUp]
        public void SetUp()
        {
            _context = new HalClientTestContext();
        }

        [Test]
        public void Navigate_to_root_resource()
        {
            _context.ArrangeHomeResource();

            _context.Act(sut => sut.Root(HalClientTestContext.RootUri));

            _context.AssertThatRootResourceIsPresent();
        }

        [Test]
        public void Navigate_to_single_resource()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            Func<IHalClient, IHalClient> act = sut =>
                sut
                    .Root(HalClientTestContext.RootUri)
                    .Get("order", new {orderRef = _context.OrderRef}, HalClientTestContext.Curie);
            _context.Act(act);

            _context.AssertThatSingleResourceIsPresent();
        }

        [Test]
        public void Navigate_to_single_embedded_resource()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            Func<IHalClient, IHalClient> act = sut =>
                sut
                    .Root(HalClientTestContext.RootUri)
                    .Get("order", new {orderRef = _context.OrderRef}, HalClientTestContext.Curie)
                    .Get("orderitem", HalClientTestContext.Curie);
            _context.Act(act);

            _context.AssertThatSingleEmbeddedResourceIsPresent();
        }

        [Test]
        public void Navigate_to_paged_resource()
        {
            _context
                .ArrangeHomeResource()
                .ArrangePagedResource();

            Func<IHalClient, IHalClient> act = sut =>
                sut
                    .Root(HalClientTestContext.RootUri)
                    .Get("order-queryby-user", new {userRef = HalClientTestContext.UserRef}, HalClientTestContext.Curie);
            _context.Act(act);

            _context.AssertThatPagedResourceIsPresent();
        }

        [Test]
        public void Navigate_to_paged_embedded_resource()
        {
            _context
                .ArrangeHomeResource()
                .ArrangePagedResource();

            Func<IHalClient, IHalClient> act = sut =>
                sut
                    .Root(HalClientTestContext.RootUri)
                    .Get("order-queryby-user", new {userRef = HalClientTestContext.UserRef}, HalClientTestContext.Curie)
                    .Get("order", HalClientTestContext.Curie);
            _context.Act(act);

            _context.AssertThatEmbeddedPagedResourceIsPresent();
        }

        [Test]
        public void Create_resource()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource()
                .ArrangeCreatedResource();

            Func<IHalClient, IHalClient> act = sut =>
                sut
                    .Root(HalClientTestContext.RootUri)
                    .Post("order-add", _context.OrderAdd, HalClientTestContext.Curie);
            _context.Act(act);

            _context.AssertThatResourceWasCreated();
        }

        [Test]
        public void Create_resource_with_parameters()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource()
                .ArrangeCreatedResource();

            Func<IHalClient, IHalClient> act = sut =>
                sut
                    .Root(HalClientTestContext.RootUri)
                    .Post("order-add", _context.OrderAdd, new {orderRef = _context.OrderRef}, HalClientTestContext.Curie);
            _context.Act(act);

            _context.AssertThatResourceWasCreated();
        }

        [Test]
        public void Update_resource()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource()
                .ArrangeUpdatedResource();

            Func<IHalClient, IHalClient> act = sut =>
                sut
                    .Root(HalClientTestContext.RootUri)
                    .Get("order", new {orderRef = _context.OrderRef}, HalClientTestContext.Curie)
                    .Put("order-edit", _context.OrderEdit, HalClientTestContext.Curie);
            _context.Act(act);

            _context.AssertThatResourceWasUpdated();
        }

        [Test]
        public void Update_resource_with_parameters()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource()
                .ArrangeUpdatedResource();

            Func<IHalClient, IHalClient> act = sut =>
                sut
                    .Root(HalClientTestContext.RootUri)
                    .Put("order-edit", _context.OrderEdit, new {orderRef = _context.OrderRef}, HalClientTestContext.Curie);
            _context.Act(act);

            _context.AssertThatResourceWasUpdated();
        }

        [Test]
        public void Delete_resource()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource()
                .ArrangeDeletedResource();

            Func<IHalClient, IHalClient> act = sut =>
                sut
                    .Root(HalClientTestContext.RootUri)
                    .Get("order", new {orderRef = _context.OrderRef}, HalClientTestContext.Curie)
                    .Delete("order-delete", HalClientTestContext.Curie);
            _context.Act(act);

            _context.AssertThatResourceWasDeleted();
        }

        [Test]
        public void Delete_resource_with_parameters()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeDeletedResource();

            Func<IHalClient, IHalClient> act = sut =>
                sut
                    .Root(HalClientTestContext.RootUri)
                    .Delete("order-delete", new {orderRef = _context.OrderRef}, HalClientTestContext.Curie);
            _context.Act(act);

            _context.AssertThatResourceWasDeleted();
        }

        [Test]
        public void Has_relationship()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            Func<IHalClient, IHalClient> act = sut =>
                sut
                    .Root(HalClientTestContext.RootUri)
                    .Get("order", new {orderRef = _context.OrderRef}, HalClientTestContext.Curie);
            _context.Act(act);

            _context.AssertThatResourceHasRelationship();
        }

        [Test]
        public void Does_not_have_relationship()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            Func<IHalClient, IHalClient> act = sut =>
                sut
                    .Root(HalClientTestContext.RootUri)
                    .Get("order", new {orderRef = _context.OrderRef}, HalClientTestContext.Curie);
            _context.Act(act);

            _context.AssertThatResourceDoesNotHasRelationship();
        }

        [Test]
        public void Throws_exception_when_template_parameters_are_not_passed()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            Func<IHalClient, IHalClient> act = sut =>
                sut
                    .Root(HalClientTestContext.RootUri)
                    .Get("order", HalClientTestContext.Curie);

            Assert.Throws<TemplateParametersAreRequired>(() => _context.Act(act));
        }

        [Test]
        public void Throws_exception_when_relationship_does_not_exist()
        {
            _context
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            Func<IHalClient, IHalClient> act = sut =>
                sut
                    .Root(HalClientTestContext.RootUri)
                    .Get("I-do-not-exist", HalClientTestContext.Curie);

            Assert.Throws<FailedToResolveRelationship>(() => _context.Act(act));
        }

        [Test]
        public void Navigate_to_single_resource_without_curie()
        {
            _context
                .ArrangeWithoutCurie()
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            Func<IHalClient, IHalClient> act = sut =>
                sut
                    .Root(HalClientTestContext.RootUri)
                    .Get("order", new {orderRef = _context.OrderRef});
            _context.Act(act);

            _context.AssertThatSingleResourceIsPresent();
        }

        [Test]
        public void Navigate_to_default_paged_resource_without_curie()
        {
            _context
                .ArrangeWithoutCurie()
                .ArrangeHomeResource()
                .ArrangeDefaultPagedResource();

            Func<IHalClient, IHalClient> act = sut =>
                sut
                    .Root(HalClientTestContext.RootUri)
                    .Get("order-query-all");
            _context.Act(act);

            _context.AssertThatPagedResourceIsPresent();
        }

        [Test]
        public void Create_resource_without_curie()
        {
            _context
                .ArrangeWithoutCurie()
                .ArrangeHomeResource()
                .ArrangeSingleResource()
                .ArrangeCreatedResource();

            Func<IHalClient, IHalClient> act = sut =>
                sut
                    .Root(HalClientTestContext.RootUri)
                    .Post("order-add", _context.OrderAdd);
            _context.Act(act);

            _context.AssertThatResourceWasCreated();
        }

        [Test]
        public void Update_resource_without_curie()
        {
            _context
                .ArrangeWithoutCurie()
                .ArrangeHomeResource()
                .ArrangeSingleResource()
                .ArrangeUpdatedResource();

            Func<IHalClient, IHalClient> act = sut =>
                sut
                    .Root(HalClientTestContext.RootUri)
                    .Get("order", new {orderRef = _context.OrderRef})
                    .Put("order-edit", _context.OrderEdit);
            _context.Act(act);

            _context.AssertThatResourceWasUpdated();
        }

        [Test]
        public void Delete_resource_without_curie()
        {
            _context
                .ArrangeWithoutCurie()
                .ArrangeHomeResource()
                .ArrangeSingleResource()
                .ArrangeDeletedResource();

            Func<IHalClient, IHalClient> act = sut =>
                sut
                    .Root(HalClientTestContext.RootUri)
                    .Get("order", new {orderRef = _context.OrderRef})
                    .Delete("order-delete");
            _context.Act(act);

            _context.AssertThatResourceWasDeleted();
        }

        [Test]
        public void Has_relationship_without_curie()
        {
            _context
                .ArrangeWithoutCurie()
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            Func<IHalClient, IHalClient> act = sut =>
                sut
                    .Root(HalClientTestContext.RootUri)
                    .Get("order", new {orderRef = _context.OrderRef});
            _context.Act(act);

            _context.AssertThatResourceHasRelationshipWithoutCurie();
        }

        [Test]
        public void Does_not_have_relationship_without_curie()
        {
            _context
                .ArrangeWithoutCurie()
                .ArrangeHomeResource()
                .ArrangeSingleResource();

            Func<IHalClient, IHalClient> act = sut =>
                sut
                    .Root(HalClientTestContext.RootUri)
                    .Get("order", new {orderRef = _context.OrderRef});
            _context.Act(act);

            _context.AssertThatResourceDoesNotHasRelationshipWithoutCurie();
        }

        [Test]
        public void HalClient_can_be_created_with_specifed_HttpClient()
        {
            _context.AssertThatHttpClientCanBeProvided();
        }

        [Test]
        public void HalClient_can_be_created_with_default_HttpClient()
        {
            _context.AssertThatDefaultHttpClientCanBeUsed();
        }

        [Test]
        public void Resolving_resource_throws_an_exception_when_the_resource_has_not_been_navigated()
        {
            _context.AssertThatResolvingResourceThrowsExceptionWhenResourceNotNavigated();
        }

        [Test]
        public void Throws_exception_when_HTTP_request_is_unsuccessful()
        {
            _context.ArrangeFailedHomeRequest();

            Func<IHalClient, IHalClient> act = sut =>
                sut.Root(HalClientTestContext.RootUri);

            Assert.Throws<HttpRequestException>(() => _context.Act(act));
        }

        [Test]
        public void Navigate_to_default_home_resource()
        {
            _context
                .ArrangeDefaultHomeResource()
                .ArrangeSingleResource();

            Func<IHalClient, IHalClient> act = sut =>
                sut
                    .Root()
                    .Get("order", new {orderRef = _context.OrderRef}, HalClientTestContext.Curie);
            _context.Act(act);

            _context.AssertThatSingleResourceIsPresent();
        }
    }
}