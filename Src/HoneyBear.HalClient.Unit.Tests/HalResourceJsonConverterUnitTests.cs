using AutoFixture;
using AutoFixture.AutoRhinoMock;
using FluentAssertions;
using HoneyBear.HalClient.Models;
using HoneyBear.HalClient.Serialization;
using Newtonsoft.Json;
using NUnit.Framework;

namespace HoneyBear.HalClient.Unit.Tests
{
    [TestFixture]
    internal class HalResourceJsonConverterUnitTests
    {
        private TestContext _context;

        [SetUp]
        public void SetUp()
        {
            _context = new TestContext();
        }

        [Test]
        public void Writing_JSON_makes_no_difference_to_the_serialization()
        {
            _context.Act();
        }

        [Test]
        public void Can_convert_Hal_resource()
        {
            _context.CanConvertHalResource();
        }

        private class TestContext
        {
            private readonly JsonWriter _writer;
            private readonly JsonSerializer _serializer;
            private readonly HalResourceJsonConverter _sut;
            private readonly object _obj;

            public TestContext()
            {
                var fixture = new Fixture().Customize(new AutoRhinoMockCustomization());

                _writer = fixture.Create<JsonWriter>();
                _obj = fixture.Create<object>();
                _serializer = fixture.Create<JsonSerializer>();

                _sut = fixture.Create<HalResourceJsonConverter>();
            }

            public void Act()
            {
                _sut.WriteJson(_writer, _obj, _serializer);
            }

            public void CanConvertHalResource()
            {
                _sut.CanConvert(typeof (Resource)).Should().BeTrue();
            }
        }
    }
}