using System;

namespace HoneyBear.HalClient.Models
{
    internal sealed class NoActiveResource : Exception
    {
        public NoActiveResource()
            : base("No active resource; you must successfully navigate to a resource before using this command.")
        {

        }
    }
}