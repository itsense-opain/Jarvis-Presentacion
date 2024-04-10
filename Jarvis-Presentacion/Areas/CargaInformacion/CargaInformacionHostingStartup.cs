using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Opain.Jarvis.Presentacion.Web.Areas.CargaInformacion.CargaInformacionHostingStartup))]
namespace Opain.Jarvis.Presentacion.Web.Areas.CargaInformacion
{
    public class CargaInformacionHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });

        }
    }
}
