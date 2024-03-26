using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Opain.Jarvis.Presentacion.Web.Areas.Informes.InformesHostingStartup))]
namespace Opain.Jarvis.Presentacion.Web.Areas.Informes
{
    public class InformesHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });

        }
    }
}
