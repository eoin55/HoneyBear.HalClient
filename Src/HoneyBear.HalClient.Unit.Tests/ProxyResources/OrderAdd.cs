using System.Collections.Generic;

namespace HoneyBear.HalClient.Unit.Tests.ProxyResources
{
    internal class OrderAdd
    {
        public IEnumerable<OrderItemAdd> OrderItems { get; set; }
    }
}