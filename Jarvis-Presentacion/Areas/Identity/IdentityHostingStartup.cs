using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Opain.Jarvis.Presentacion.Web.Areas.Identity.IdentityHostingStartup))]
namespace Opain.Jarvis.Presentacion.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}