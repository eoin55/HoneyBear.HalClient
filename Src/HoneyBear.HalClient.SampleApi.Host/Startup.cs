using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HoneyBear.HalClient.SampleApi.Host
{
    internal sealed class Startup
    {
        public void ConfigureServices(IServiceCollection services)
            => services.AddMvc();

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
            => app.UseMvc();
    }
}
