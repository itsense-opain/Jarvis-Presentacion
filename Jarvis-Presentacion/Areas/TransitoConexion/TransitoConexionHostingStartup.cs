using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Opain.Jarvis.Presentacion.Web.Areas.CargaInformacion.TransitoConexionHostingStartup))]
namespace Opain.Jarvis.Presentacion.Web.Areas.CargaInformacion
{
    public class TransitoConexionHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });

        }
    }
}
