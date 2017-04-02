using Microsoft.AspNetCore.Hosting;

namespace HoneyBear.HalClient.SampleApi.Host
{
    internal sealed class Program
    {
        public static void Main(string[] args) =>
            new WebHostBuilder()
                .UseKestrel()
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build()
                .Run();
    }
}
