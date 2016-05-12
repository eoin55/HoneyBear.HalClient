using System;
using System.Collections.ObjectModel;
using NMoneys;

namespace HoneyBear.HalClient.Unit.Tests.ProxyResources
{
    internal class OrderItem
    {
        public Guid OrderItemRef { get; set; }
        public string Status { get; set; }
        public Money Total { get; set; }
        public int Quantity { get; set; }
        public ReadOnlyCollection<string> SerialNumbers { get; set; }
    }
}