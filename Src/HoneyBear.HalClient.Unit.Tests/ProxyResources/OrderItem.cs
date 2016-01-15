using System;
using NMoneys;

namespace HoneyBear.HalClient.Unit.Tests.ProxyResources
{
    internal class OrderItem
    {
        public Guid OrderItemRef { get; set; }
        public string Status { get; set; }
        public Money Total { get; set; }
        public int Quantity { get; set; }
    }
}