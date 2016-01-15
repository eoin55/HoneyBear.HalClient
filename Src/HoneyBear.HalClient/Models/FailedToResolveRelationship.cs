using System;

namespace HoneyBear.HalClient.Models
{
    internal sealed class FailedToResolveRelationship : Exception
    {
        public FailedToResolveRelationship(string relationship)
            : base($"Failed to resolve relationship:{relationship}")
        {

        }
    }
}