using System;
using NMoneys;

namespace HoneyBear.HalClient.Unit.Tests.ProxyResources
{
    internal class Order
    {
        public Guid OrderRef { get; set; }
        public string OrderNumber { get; set; }
        public string Status { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public Money Total { get; set; }
    }
}
